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
            string guid = string.Empty;

            try
            {
                AuthenticationDataSessionStorage ads = new AuthenticationDataSessionStorage();

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

                return View("/Areas/eContracting/Views/Expiration.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying expiration page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}
