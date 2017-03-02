using System;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc;
using Sitecore.Mvc.Controllers;
using Log = Sitecore.Diagnostics.Log;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    public class eContractingController : SitecoreController
    {
        public ActionResult CookieLaw()
        {
            try
            {
                using (var sitecoreContext = new SitecoreContext())
                {
                    var cookieLawSettings = sitecoreContext.GetItem<CookieLawSettings>(ItemPaths.CookieLawSettings);
                    if (cookieLawSettings != null)
                    {
                        return View("/Areas/eContracting/Views/CookieLaw.cshtml", cookieLawSettings);
                    }
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying cookie law", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        public ActionResult DocumentPanel(bool isAccepted)
        {
            try
            {
                var model = new DocumentPanelModel();

                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                model.ClientId = authenticationDataSessionStorage.GetUserData().Identifier;
                model.IsAccepted = isAccepted;

                var generalSettings = ConfigHelpers.GetGeneralSettings();

                ViewData["IAmInformed"] = generalSettings.IAmInformed;
                ViewData["IAgree"] = generalSettings.IAgree;
                ViewData["Accept"] = generalSettings.Accept;

                return View("/Areas/eContracting/Views/DocumentPanel.cshtml", model);
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying document panel.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept()
        {
            try
            {
                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                string guid = authenticationDataSessionStorage.GetUserData().Identifier;
                RweClient client = new RweClient();
                if (client.GuidExistInMongo(guid))
                {
                    var acceptOfferUrl = ConfigHelpers.GetPageLink(PageLinkType.AcceptedOffer).Url;
                    authenticationDataSessionStorage.GetUserData().IsAccepted = true;
                    return Redirect(acceptOfferUrl);
                }

                client.AcceptOffer(authenticationDataSessionStorage.GetUserData().Identifier);  
                authenticationDataSessionStorage.GetUserData().IsAccepted = true;


                var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.ThankYou).Url;
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Log.Error("Error when accepting offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}