using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Handles authentication process for user.
    /// </summary>
    public class WelcomeController : BaseController<EContractingWelcomeTemplate>
    {
        /// <summary>
        /// Authentication GET action.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpGet]
        public ActionResult Welcome()
        {
            try
            {
                var guid = Request.QueryString["guid"];

                var dataModel = new WelcomeModel() { Guid = guid };
                return View("/Areas/eContracting/Views/Welcome.cshtml", dataModel);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying welcome user page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        [HttpPost]
        public ActionResult WelcomeSubmit(string guid)
        {
            return RedirectToAction("Authentication", "eContractingAuthentication", new { guid = guid });
        }
    }
}
