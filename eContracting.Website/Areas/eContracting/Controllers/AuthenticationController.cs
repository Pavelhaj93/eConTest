using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Models;
using Glass.Mapper.Sc.Web.Mvc;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class AuthenticationController : GlassController<EContractingAuthenticationTemplate>
    {
        [HttpGet]
        public ActionResult Authentication()
        {
            return View("/Areas/eContracting/Views/Authentication.cshtml");
        }

        [HttpPost]
        public ActionResult Authentication(AuthenticationModel model)
        {
            return View("/Areas/eContracting/Views/Error404.cshtml");
        }
    }
}