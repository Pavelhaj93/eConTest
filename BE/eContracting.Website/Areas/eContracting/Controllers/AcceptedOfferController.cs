using System;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Utils;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Controller for accepted offer.
    /// </summary>
    public class AcceptedOfferController : BaseController<EContractingAcceptedOfferTemplate>
    {
        /// <summary>
        /// Handler for AcceptedOffer action.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpGet]
        public ActionResult AcceptedOffer()
        {
            string guid = string.Empty;

            try
            {
                var ads = new AuthenticationDataSessionStorage();

                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var data = ads.GetUserData();
                guid = data.Identifier;
                var textHelper = new EContractingTextHelper(SystemHelpers.GenerateMainText);
                var mainText = textHelper.GetMainText(this.Context, data);

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = mainText;

                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;
                ViewData["SignFailure"] = generalSettings.SignFailure;

                return View("/Areas/eContracting/Views/AcceptedOffer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying accepted offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}
