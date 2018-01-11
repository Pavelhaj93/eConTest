using System;
using System.Collections.Generic;
using System.IO;
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
    public class AuthenticationController : BaseController<EContractingAuthenticationTemplate>
    {
        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
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

                if (offer.OfferInternal.State == "1" || offer.OfferInternal.State == "3")
                {
                    client.ReadOffer(guid);
                }

                if (CheckWhetherUserIsBlocked(guid))
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);


                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var authenticationData = authenticationDataSessionStorage.GetUserData(offer, true);

                dataModel.ItemValue = authenticationData.ItemValue.Trim().Replace(" ", String.Empty).ToLower().GetHashCode().ToString();

                string contentText = authenticationData.IsAccepted ? Context.AcceptedOfferText : Context.NotAcceptedOfferText;
                string maintext = SystemHelpers.GenerateMainText(authenticationData, contentText);
                if (maintext == null)
                {
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = maintext;

                ViewData["AdditionalPlaceholder"] = string.Format(this.Context.ContractDataPlaceholder, authenticationData.ItemFriendlyName);

                if (offer.OfferInternal.HasGDPR)
                {
                    var GDPRGuid = AesEncrypt(offer.OfferInternal.GDPRKey, Context.AesEncryptKey, Context.AesEncryptVector);

                    ViewData["GDPRGuid"] = GDPRGuid;
                    ViewData["GDPRUrl"] = Context.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }

                return View("/Areas/eContracting/Views/Authentication.cshtml", dataModel);
            }
            catch (Exception ex)
            {
                Log.Error("Error when authenticating user", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        /// <summary>
        /// Checks whether user is not blocked.
        /// </summary>
        /// <param name="guid">GUID of offer we are checking.</param>
        /// <returns>A value indicating whether user is blocked or not.</returns>
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

        /// <summary>
        /// Processes user's authentication.
        /// </summary>
        /// <param name="authenticationModel">Authentication model.</param>
        /// <returns>Instance result.</returns>
        [HttpPost]
        public ActionResult Authentication(AuthenticationModel authenticationModel)
        {
            try
            {
                FillViewData();
                var dobValue = authenticationModel.BirthDate.Trim().Replace(" ", string.Empty).ToLower();
                var additionalValue = authenticationModel.Additional.Trim().Replace(" ", String.Empty).ToLower().GetHashCode().ToString();

                RweClient client = new RweClient();
                var offer = client.GenerateXml(Request.QueryString["guid"]);
                AuthenticationDataSessionStorage ass = new AuthenticationDataSessionStorage();
                AuthenticationDataItem authenticationData = ass.GetUserData(offer, false);
                authenticationData.ItemValue = authenticationModel.ItemValue;

                failureData failureData;
                if (!this.NumberOfLogons.TryGetValue(authenticationData.Identifier, out failureData))
                {
                    failureData = new failureData();
                    NumberOfLogons.Add(authenticationData.Identifier, failureData);
                    failureData.LastFailureTime = DateTime.UtcNow;
                    failureData.LoginAttemp = 0;
                }
                int numberOfLogonsBefore = failureData.LoginAttemp;

                if ((numberOfLogonsBefore < 3) && ((dobValue != authenticationData.DateOfBirth.Trim().Replace(" ", String.Empty).ToLower()) || (additionalValue != authenticationData.ItemValue)))
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
                if (System.Web.HttpContext.Current.Application["NumberOfLogons"] == null)
                {
                    System.Web.HttpContext.Current.Application["NumberOfLogons"] = new Dictionary<string, failureData>();
                }
                return (Dictionary<string, failureData>)System.Web.HttpContext.Current.Application["NumberOfLogons"];
            }
        }

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

    internal class failureData
    {
        public int LoginAttemp { get; set; }
        public DateTime LastFailureTime { get; set; }

    }

}