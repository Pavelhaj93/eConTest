using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Settings;
using Glass.Mapper.Sc;
using Sitecore.Mvc.Controllers;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class eContractingController : SitecoreController
    {
        public ActionResult CookieLaw()
        {
            using (var sitecoreContext = new SitecoreContext())
            {
                var cookieLawSettings = sitecoreContext.GetItem<CookieLawSettings>(ItemIds.CookieLawSettings);
                if (cookieLawSettings != null)
                {
                    return View("/Areas/eContracting/Views/CookieLaw.cshtml", cookieLawSettings);
                }
            }

            return new EmptyResult();
        }
    }
}