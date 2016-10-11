using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class UserBlockedController : GlassController<EContractingUserBlockedTemplate>
    {
         public ActionResult UserBlocked()
        {
            if (Sitecore.Context.PageMode.IsExperienceEditor)
            {
                return View("/Areas/eContracting/Views/UserBlocked.cshtml", Context);
            }

            try
            {
                Session["NumberOfLogons"] = 0;
                return View("/Areas/eContracting/Views/UserBlocked.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying user blocked page", ex, this);
                return View("/Areas/eContracting/Views/SystemError.cshtml");
            }
        }
    }
}