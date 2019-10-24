using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class eContractingAuthenticationController : BaseController<EContractingAuthenticationTemplate>
    {
        private const string salt = "228357";
        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpGet]
        public ActionResult Authentication()
        {
            try
            {
                var guid = Request.QueryString["guid"];

                if (string.IsNullOrEmpty(guid))
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                if (this.CheckWhetherUserIsBlocked(guid))
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                }

                var client = new RweClient();
                var offer = client.GenerateXml(guid);

                if ((offer == null) || (offer.OfferInternal.Body == null) || string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                var authSettings = ConfigHelpers.GetAuthenticationSettings();
                var authHelper = new AuthenticationHelper(offer, new AuthenticationDataSessionStorage(), this.Context.UserChoiceAuthenticationEnabled, this.Context.UserChoiceAuthenticationEnabledRetention, authSettings);
                var userData = authHelper.GetUserData();

                if (!this.Context.WelcomePageEnabled && (offer.OfferInternal.State == "1" || offer.OfferInternal.State == "3"))
                {
                    client.ReadOffer(guid);
                }

                if (this.Request.QueryString["fromWelcome"] != "1" && !userData.IsAccepted)
                {
                    if (this.Context.WelcomePageEnabled && !userData.IsRetention)
                    {
                        var welcomeRedirectUrl = ConfigHelpers.GetPageLink(PageLinkType.Welcome).Url + "?guid=" + guid;
                        return Redirect(welcomeRedirectUrl);
                    }
                    else if (this.Context.WelcomePageEnabledRetention && userData.IsRetention)
                    {
                        var welcomeRedirectUrl = ConfigHelpers.GetPageLink(PageLinkType.Welcome).Url + "?guid=" + guid;
                        return Redirect(welcomeRedirectUrl);
                    }
                    else
                    {
                        if (offer.OfferInternal.State == "1" || offer.OfferInternal.State == "3")
                        {
                            client.ReadOffer(guid);
                        }
                    }
                }

                var dataModel = new AuthenticationModel();

                if (authHelper.IsUserChoice)
                {
                    dataModel.IsUserChoice = true;
                    var items = new List<AuthenticationSelectListItem>();
                    var available = authHelper.GetAvailableAuthenticationFields();
                    foreach (var item in available)
                    {
                        items.Add(new AuthenticationSelectListItem {DataHelpValue = item.Hint, Value = item.AuthenticationDFieldName, Text = item.UserFriendlyName });
                    }

                    dataModel.AvailableFields = items;
                }
                else
                {
                    var value = userData.ItemValue.Trim().Replace(" ", string.Empty).ToLower().GetHashCode().ToString();
                    dataModel.ItemValue = string.Format("{0}{1}", value, salt);     ////hash + salt
                    dataModel.IsUserChoice = false;
                }

                var contentText = userData.IsAccepted ? this.Context.AcceptedOfferText : this.Context.NotAcceptedOfferText;
                var maintext = SystemHelpers.GenerateMainText(userData, contentText, string.Empty);

                if (maintext == null)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                }

                FillViewData(maintext, string.Format(this.Context.ContractDataPlaceholder, userData.ItemFriendlyName));

                return View("/Areas/eContracting/Views/Authentication.cshtml", dataModel);
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        /// <summary>
        /// Processes user's authentication.
        /// </summary>
        /// <param name="authenticationModel">Authentication model.</param>
        /// <returns>Instance result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authentication(AuthenticationModel authenticationModel)
        {
            try
            {
                var guid = Request.QueryString["guid"];

                if (this.CheckWhetherUserIsBlocked(guid))
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                }

                FillViewData();

                var client = new RweClient();
                var offer = client.GenerateXml(Request.QueryString["guid"]);

                var authSettings = ConfigHelpers.GetAuthenticationSettings();
                var authHelper = new AuthenticationHelper(offer, new AuthenticationDataSessionStorage(), this.Context.UserChoiceAuthenticationEnabled, this.Context.UserChoiceAuthenticationEnabledRetention, authSettings);
                var userData = authHelper.GetUserData();

                var dateOfBirthRealValue = userData.DateOfBirth.Trim().Replace(" ", string.Empty).ToLower();                    ////Value from offer
                var dateOfBirthUserValue = authenticationModel.BirthDate.Trim().Replace(" ", string.Empty).ToLower();           ////Value from user

                var additionalUserValue = authenticationModel.Additional.Trim().Replace(" ", string.Empty).ToLower().GetHashCode().ToString();      ////Value from user hashed
                var additionalRealValue = authHelper.IsUserChoice 
                    ? authHelper.GetRealAdditionalValue(authenticationModel.SelectedKey) 
                    : authenticationModel.ItemValue.Replace(salt, string.Empty);     ////Value from offer hashed - salt

                var validFormat = (!string.IsNullOrEmpty(dateOfBirthRealValue)) && (!string.IsNullOrEmpty(dateOfBirthUserValue)) && (!string.IsNullOrEmpty(additionalUserValue)) && (!string.IsNullOrEmpty(additionalRealValue));
                var validData = (dateOfBirthUserValue == dateOfBirthRealValue) && (additionalUserValue == additionalRealValue);

                if (!validFormat || !validData)
                {
                    var siteSettings = ConfigHelpers.GetSiteSettings();
                    var loginsCheckerClient = new LoginsCheckerClient(siteSettings.MaxFailedAttempts, siteSettings.DelayAfterFailedAttemptsTimeSpan);
                    loginsCheckerClient.AddFailedAttempt(guid, this.Session.SessionID, Request.Browser.Browser);
                    var url = Request.RawUrl.Contains("&error=validationError") ? Request.RawUrl : Request.RawUrl + "&error=validationError";

                    return Redirect(url);
                }

                if (!ModelState.IsValid)
                {
                    return View("/Areas/eContracting/Views/Authentication.cshtml", authenticationModel);
                }

                var aut = new AuthenticationDataSessionStorage();   ////why???
                aut.Login(userData);

                if (userData.IsAccepted)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    return Redirect(redirectUrl);
                }

                if (userData.OfferIsExpired)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                    return Redirect(redirectUrl);
                }

                return Redirect(Context.NextPageLink.Url);
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        private void FillViewData(string mainText = null, string additionalPlaceholder = null)
        {
            ViewData["FirstText"] = this.Context.DateOfBirth;
            ViewData["SecondText"] = this.Context.ContractData;
            ViewData["ButtonText"] = this.Context.ButtonText;
            ViewData["BirthDatePlaceholder"] = this.Context.DateOfBirthPlaceholder;
            ViewData["RequiredFields"] = this.Context.RequiredFields;
            ViewData["ValidationMessage"] = this.Context.ValidationMessage;
            ViewData["SecondContractPropertyLabel"] = this.Context.ContractSecondPropertyLabel;

            if (!string.IsNullOrEmpty(mainText))
            {
                ViewData["MainText"] = mainText;
            }

            if (!string.IsNullOrEmpty(additionalPlaceholder))
            {
                ViewData["AdditionalPlaceholder"] = additionalPlaceholder;
            }
        }

        //private Dictionary<string, failureData> NumberOfLogons
        //{
        //    get
        //    {
        //        if (System.Web.HttpContext.Current.Application["NumberOfLogons"] == null)
        //        {
        //            System.Web.HttpContext.Current.Application["NumberOfLogons"] = new Dictionary<string, failureData>();
        //        }
        //        return (Dictionary<string, failureData>)System.Web.HttpContext.Current.Application["NumberOfLogons"];
        //    }
        //}

        public static string AesEncrypt(string input, string key, string vector)
        {
            byte[] encrypted;
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Encoding.UTF8.GetBytes(key);
                rijAlg.IV = Encoding.UTF8.GetBytes(vector);

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }
    }

    //internal class failureData
    //{
    //    public int LoginAttemp { get; set; }
    //    public DateTime LastFailureTime { get; set; }

    //}

}
