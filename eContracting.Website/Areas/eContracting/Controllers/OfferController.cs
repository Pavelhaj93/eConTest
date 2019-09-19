using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
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
        public ActionResult Offer()
        {
            try
            {
                AuthenticationDataSessionStorage ads = new AuthenticationDataSessionStorage();
                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                if (ads.GetUserData().IsAccepted)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    return Redirect(redirectUrl);
                }

                if (ads.GetUserData().OfferIsExpired)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.OfferExpired).Url;
                    return Redirect(redirectUrl);
                }
                string guid = ads.GetUserData().Identifier;

                RweClient client = new RweClient();
                client.SignOffer(guid);

                var offer = client.GenerateXml(guid);

                var parameters = SystemHelpers.GetParameters(ads.GetUserData().Identifier, SystemHelpers.GetCodeOfAdditionalInfoDocument(offer));

                var mainText = string.Empty;

                if (ads.GetUserData().IsRetention)
                {
                    mainText = SystemHelpers.GenerateMainText(ads.GetUserData(), parameters, Context.MainTextRetention);
                }
                else
                {
                    mainText = SystemHelpers.GenerateMainText(ads.GetUserData(), parameters, Context.MainText);
                }

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }
                ViewData["MainText"] = mainText;

                if (ads.GetUserData().IsRetention)
                {
                    string voucherText = SystemHelpers.GenerateMainText(ads.GetUserData(), parameters, Context.VoucherText);
                    ViewData["VoucherText"] = voucherText;
                }
                else
                {
                    ViewData["VoucherText"] = null;
                }

                if (offer.OfferInternal.HasGDPR)
                {
                    var GDPRGuid = AesEncrypt(offer.OfferInternal.GDPRKey, Context.AesEncryptKey, Context.AesEncryptVector);

                    ViewData["GDPRGuid"] = GDPRGuid;
                    ViewData["GDPRUrl"] = Context.GDPRUrl + "?hash=" + GDPRGuid + "&typ=g";
                }


                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = generalSettings.SignFailure;

                return View("/Areas/eContracting/Views/Offer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
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
}
