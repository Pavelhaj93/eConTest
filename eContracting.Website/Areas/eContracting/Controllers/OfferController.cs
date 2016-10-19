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

                if (text != null && text.Any())
                {
                    var tr = new StringReader(text.First().Text);
                    XDocument doc = XDocument.Load(tr);
                    var textNode = doc.Descendants("BODY").FirstOrDefault();

                    if (textNode != null)
                    {
                        var els = textNode.FirstNode;
                        if (els != null)
                        {
                            var offerSubText = els as XElement;
                            var mainOfferText = offerSubText.Elements().FirstOrDefault();

                            if (mainOfferText != null)
                            {
                                ViewData["MainText"] = mainOfferText.ToString();
                            }
                        }
                    }
                }

                return View("/Areas/eContracting/Views/Offer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying offer.", ex, this);
                return View("/Areas/eContracting/Views/SystemError.cshtml");
            }
        }
    }
}