using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Analytics;
using Sitecore.Diagnostics;
using System.Collections.Generic;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class AuthenticationController : BaseController<EContractingAuthenticationTemplate>
    {
        [HttpGet]
        public ActionResult Authentication()
        {
            try
            {
                FillViewData();

                var dataModel = new AuthenticationModel();

                RweClient client = new RweClient();
                var guid = Request.QueryString["guid"];

                var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                if (string.IsNullOrEmpty(guid))
                {
                    return Redirect(redirectUrl);
                }

                var offer = client.GenerateXml(guid);
                if ((offer == null) || (offer.OfferInternal.Body == null) || string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                {
                    return Redirect(redirectUrl);
                }

                if (CheckWhetherUserIsBlocked(guid))
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);


                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var authenticationData = authenticationDataSessionStorage.GetUserData(offer,true);

                dataModel.ItemValue = authenticationData.ItemValue;
                dataModel.UserId = authenticationData.Identifier;

                string contentText = authenticationData.IsAccepted ? Context.AcceptedOfferText : Context.NotAcceptedOfferText;
                string maintext = SystemHelpers.GenerateMainText(authenticationData, contentText);
                if (maintext == null)
                {
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = maintext; 

                ViewData["AdditionalPlaceholder"] = string.Format(this.Context.ContractDataPlaceholder, authenticationData.ItemFriendlyName);

                return View("/Areas/eContracting/Views/Authentication.cshtml", dataModel);
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }


        private bool CheckWhetherUserIsBlocked(string guid)
        {
            failureData failureData;
            if (this.NumberOfLogons.TryGetValue(guid, out failureData))
            {
                TimeSpan blokingDuration = DateTime.UtcNow - failureData.LastFailureTime;
                if (failureData.LoginAttemp >= 3 && blokingDuration.Minutes <= 30)
                {
                    return true;
                }
            }

            return false;
        }

        [HttpPost]
        public ActionResult Authentication(AuthenticationModel authenticationModel)
        {
            try
            {
                FillViewData();
                var dobValue = authenticationModel.BirthDate.Trim().Replace(" ", string.Empty).ToLower();
                var additionalValue = authenticationModel.Additional.Trim().Replace(" ", string.Empty).ToLower();

                RweClient client = new RweClient();
                var offer = client.GenerateXml(authenticationModel.UserId);
                AuthenticationDataSessionStorage ass = new AuthenticationDataSessionStorage();
                AuthenticationDataItem authenticationData = ass.GetUserData(offer,false);
                authenticationData.ItemValue = authenticationModel.ItemValue;

                failureData failureData;
                if(! this.NumberOfLogons.TryGetValue(authenticationData.Identifier, out failureData))
                {
                    failureData = new failureData();
                    NumberOfLogons.Add(authenticationData.Identifier, failureData);
                    failureData.LastFailureTime = DateTime.UtcNow;
                    failureData.LoginAttemp = 0;
                }
                int numberOfLogonsBefore = failureData.LoginAttemp;

                if ((numberOfLogonsBefore < 3) && ((dobValue != authenticationData.DateOfBirth.Trim().Replace(" ", String.Empty).ToLower()) || (additionalValue != authenticationData.ItemValue.Trim().Replace(" ", String.Empty).ToLower())))
                {

                    NumberOfLogons[authenticationData.Identifier].LoginAttemp = ++numberOfLogonsBefore;
                    NumberOfLogons[authenticationData.Identifier].LastFailureTime = DateTime.UtcNow;

                    string url = Request.RawUrl;

                    if (Request.RawUrl.Contains("&error=validationError"))
                    {
                        url = Request.RawUrl;
                    }
                    else
                    {
                        url = Request.RawUrl + "&error=validationError";
                    }

                    return Redirect(url);
                }

                if (CheckWhetherUserIsBlocked(authenticationData.Identifier))
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                else
                {
                    if (!ModelState.IsValid)
                    {
                        return View("/Areas/eContracting/Views/Authentication.cshtml", authenticationModel);
                    }
                    NumberOfLogons.Remove(authenticationData.Identifier);
                    var aut = new AuthenticationDataSessionStorage();
                    aut.Login(authenticationData);

                    if (authenticationData.IsAccepted)
                    {
                        var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                        return Redirect(redirectUrl);
                    }

                    if (authenticationData.OfferIsExpired)
                    {
                        var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                        return Redirect(redirectUrl);
                    }

                    return Redirect(Context.NextPageLink.Url);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        private void FillViewData()
        {
            ViewData["FirstText"] = this.Context.DateOfBirth;
            ViewData["SecondText"] = this.Context.ContractData;
            ViewData["ButtonText"] = this.Context.ButtonText;
            ViewData["BirthDatePlaceholder"] = this.Context.DateOfBirthPlaceholder;

            ViewData["RequiredFields"] = this.Context.RequiredFields;
            ViewData["ValidationMessage"] = this.Context.ValidationMessage;
        }

        private Dictionary<string, failureData> NumberOfLogons
        {
            get
            {
                if(System.Web.HttpContext.Current.Application["NumberOfLogons"] == null)
                {
                    System.Web.HttpContext.Current.Application["NumberOfLogons"] = new Dictionary<string, failureData>();
                }
                return (Dictionary<string, failureData>)System.Web.HttpContext.Current.Application["NumberOfLogons"];
            }
        }
    }

    internal class failureData
    {
        public int LoginAttemp { get; set; }
        public DateTime LastFailureTime { get; set; }

    }

}