using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Utils;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Expiration related controllers.
    /// </summary>
    public class ExpirationController : BaseController<EContractingExpirationTemplate>
    {
        /// <summary>
        /// Expiration page.
        /// </summary>
        /// <returns>Instance result.</returns>
        public ActionResult Expiration()
        {
            try
            {
                AuthenticationDataSessionStorage ads = new AuthenticationDataSessionStorage();

                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var mainText = string.Empty;
                if (ads.GetUserData().IsRetention)
                {
                    mainText = SystemHelpers.GenerateMainText(ads.GetUserData(), Context.MainTextRetention, string.Empty);
                }
                else
                {
                    mainText = SystemHelpers.GenerateMainText(ads.GetUserData(), Context.MainText, string.Empty);
                }

                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = mainText;


                return View("/Areas/eContracting/Views/Expiration.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying expiration page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}
