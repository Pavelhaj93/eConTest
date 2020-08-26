using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// User blocked functionalities.
    /// </summary>
    public class UserBlockedController : BaseController<EContractingUserBlockedTemplate>
    {
        /// <summary>
        /// User blocked page.
        /// </summary>
        /// <returns>Instance result.</returns>
        [Obsolete("Use 'eContracting2Controller.UserBlocked' instead")]
        public ActionResult UserBlocked()
        {
            try
            {
                return View("/Areas/eContracting/Views/UserBlocked.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying user blocked page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}
