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
            string mainText = string.Empty;
            try
            {
                AuthenticationDataSessionStorage ads = new AuthenticationDataSessionStorage();

                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                mainText = SystemHelpers.GenerateMainText(ads.GetUserData(), Context.MainText);
                if (mainText == null)
                {
                    var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url;
                    return Redirect(redirectUrl);
                }
                RweClient client = new RweClient();
                client.RemoveFllesFromMongo();
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