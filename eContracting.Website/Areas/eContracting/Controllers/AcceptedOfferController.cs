using System;
using System.Web.Mvc;
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
            try
            {
                AuthenticationDataSessionStorage ads = new AuthenticationDataSessionStorage();

                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }
                string maintext = SystemHelpers.GenerateMainText(ads.GetUserData(), Context.MainText);
                if (maintext == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = maintext;

                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;

                return View("/Areas/eContracting/Views/AcceptedOffer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying accepted offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}