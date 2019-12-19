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
    /// Thank you page.
    /// </summary>
    public class ThankYouController : BaseController<EContractingThankYouTemplate>
    {
        /// <summary>
        /// Thank you page.
        /// </summary>
        /// <returns>Instance result.</returns>
        public ActionResult ThankYou()
        {
            string mainText = string.Empty;
            try
            {
                var ads = new AuthenticationDataSessionStorage();
                var data = ads.GetUserData();

                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                if (data.OfferType == OfferTypes.Retention)
                {
                    mainText = SystemHelpers.GenerateMainText(data, Context.MainTextRetention, string.Empty);
                }
                else if (data.OfferType == OfferTypes.Acquisition)
                {
                    mainText = SystemHelpers.GenerateMainText(data, Context.MainTextAcquisition, string.Empty);
                }
                else
                {
                    mainText = SystemHelpers.GenerateMainText(data, Context.MainText, string.Empty);
                }


                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }
                Session.Remove("UserFiles");
            }
            catch (Exception ex)
            {
                mainText = Context.ServiceUnavailableText;
                Log.Error("Error when displaying thank you page", ex, this);
            }
            ViewData["MainText"] = mainText;
            return View("/Areas/eContracting/Views/ThankYou.cshtml", Context);
        }
    }
}
