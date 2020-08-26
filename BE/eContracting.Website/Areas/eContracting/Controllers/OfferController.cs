using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Offer related action methods.
    /// </summary>
    public class OfferController : BaseController<EContractingOfferTemplate>
    {
        /// <summary>
        /// Offer action.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpGet]
        [Obsolete("Use 'eContracting2Controller.Offer' instead")]
        public ActionResult Offer()
        {
            string guid = string.Empty;

            try
            {
                var ads = new AuthenticationDataSessionStorage();

                if (!ads.IsDataActive)
                {
                    Log.Debug($"[{guid}] Session expired", this);
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var data = ads.GetUserData();
                guid = data.Identifier;

                if (data.IsAccepted)
                {
                    Log.Debug($"[{guid}] Offer already accepted", this);
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    return Redirect(redirectUrl);
                }

                if (data.OfferIsExpired)
                {
                    Log.Debug($"[{guid}] Offer expired", this);
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                    return Redirect(redirectUrl);
                }

                RweClient client = new RweClient();
                client.SignOffer(guid);

                var offer = client.GenerateXml(guid);
                var parameters = SystemHelpers.GetParameters(data.Identifier, data.OfferType, SystemHelpers.GetCodeOfAdditionalInfoDocument(offer));
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText, parameters);
                var mainText = textHelper.GetMainText(this.Context, data);

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                this.ViewData["MainText"] = mainText;
                this.ViewData["VoucherText"] = textHelper.GetVoucherText(this.Context, data);

                if (offer.OfferInternal.HasGDPR)
                {
                    var GDPRGuid = AesEncrypt(offer.OfferInternal.GDPRKey, Context.AesEncryptKey, Context.AesEncryptVector);

                    ViewData["GDPRGuid"] = GDPRGuid;
                    ViewData["GDPRUrl"] = Context.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }


                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = generalSettings.GetSignInFailure(data.OfferType);

                return View("/Areas/eContracting/Views/Offer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        [Obsolete("Use 'StringUtils.AesEncrypt' instead")]
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
}
