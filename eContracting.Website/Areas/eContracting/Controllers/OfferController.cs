using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class OfferController : BaseController<EContractingOfferTemplate>
    {
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


                string maintext = SystemHelpers.GenerateMainText(ads.GetUserData(), Context.MainText);
                if (maintext == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }

                ViewData["MainText"] = maintext;

                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;

                return View("/Areas/eContracting/Views/Offer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}