using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Diagnostics;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class OfferController : GlassController<EContractingOfferTemplate>
    {
        [HttpGet]
        public ActionResult Offer()
        {
            try
            {
                RweUtils utils = new RweUtils();
                if (!utils.IsUserInSession())
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var data = authenticationDataSessionStorage.GetData();

                RweClient client = new RweClient();
                var text = client.GetTextsXml(data.Identifier);
                var letterXml = client.GetLetterXml(text);
                ViewData["MainText"] = client.GetAttributeText("BODY", letterXml);

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