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
    public class ThankYouController : BaseController<EContractingThankYouTemplate>
    {
        public ActionResult ThankYou()
        {
            try
            {
                AuthenticationDataSessionStorage ads = new AuthenticationDataSessionStorage();
                RweClient client = new RweClient();

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

                return View("/Areas/eContracting/Views/ThankYou.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying thank you page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}