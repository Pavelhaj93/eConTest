using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class ExpirationController : GlassController<EContractingExpirationTemplate>
    {
        public ActionResult Expiration()
        {
            try
            {
                if (Sitecore.Context.PageMode.IsExperienceEditor)
                {
                    return View("/Areas/eContracting/Views/Expiration.cshtml", Context);
                }

                RweUtils utils = new RweUtils();
                if (!utils.IsUserInSession())
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var data = authenticationDataSessionStorage.GetData();
                try
                {
                    Context.MainText = string.Format(Context.MainText, data.LastName, data.ExpDateFormatted);
                }
                catch (Exception ex)
                {
                    Log.Warn("Error when processing users name pattern in expiration page", this);
                }

                return View("/Areas/eContracting/Views/Expiration.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying expiration page", ex, this);
                return View("/Areas/eContracting/Views/SystemError.cshtml");
            }
        }
    }
}