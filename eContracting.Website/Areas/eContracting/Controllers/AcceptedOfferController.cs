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
    public class AcceptedOfferController : GlassController<EContractingAcceptedOfferTemplate>
    {
        [HttpGet]
        public ActionResult AcceptedOffer()
        {
            try
            {
                RweUtils utils = new RweUtils();
                RweClient client = new RweClient();

                if (!utils.IsUserInSession())
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var data = authenticationDataSessionStorage.GetData();

                var text = client.GetTextsXml(data.Identifier);
                var letterXml = client.GetLetterXml(text);
                var salutation = client.GetAttributeText("CUSTTITLELET", letterXml);
                var date = client.GenerateXml(data.Identifier);

                ViewData["MainText"] = Context.MainText.Replace("{SALUTATION}", salutation).Replace("{DATE}", date.OfferInternal.AcceptedAt);

                var generalSettings = ConfigHelpers.GetGeneralSettings();
                ViewData["AppNotAvailable"] = generalSettings.AppNotAvailable;

                return View("/Areas/eContracting/Views/AcceptedOffer.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying accepted offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}