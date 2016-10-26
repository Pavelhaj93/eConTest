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
    public class ExpirationController : GlassController<EContractingExpirationTemplate>
    {
        public ActionResult Expiration()
        {
            try
            {
                RweUtils utils = new RweUtils();
                RweClient client = new RweClient();

                if (!utils.IsUserInSession())
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                AuthenticationDataSessionStorage authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                var data = authenticationDataSessionStorage.GetData();

                var text = client.GetTextsXml(data.Identifier);
                var letterXml = client.GetLetterXml(text);
                var salutation = client.GetAttributeText("CUSTTITLELET", letterXml);
                var date = data.ExpDateFormatted;

                ViewData["MainText"] = Context.MainText.Replace("{SALUTATION}", salutation).Replace("{DATE}", date);

                return View("/Areas/eContracting/Views/Expiration.cshtml", Context);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying expiration page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}