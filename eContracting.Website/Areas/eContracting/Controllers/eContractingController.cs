using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.GlassItems.Content.Modal_window;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc;
using Sitecore.Mvc.Controllers;
using Log = Sitecore.Diagnostics.Log;
using Sitecore.Mvc.Presentation;

namespace eContracting.Website.Areas.eContracting.Controllers
{
    /// <summary>
    /// Controller used for basic steps and operations.
    /// </summary>
    public class eContractingController : SitecoreController
    {
        /// <summary>
        /// Cookie law action.
        /// </summary>
        /// <returns>Instance result.</returns>
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

        /// <summary>
        /// Rendering of document panel.
        /// </summary>
        /// <param name="isAccepted">Flag indicating whether offer is already accepted or not.</param>
        /// <returns>Instance result.</returns>
        public ActionResult DocumentPanel(bool isAccepted)
        {
            //// {CE5332E3-21B0-419D-BE64-FAD155123E42}
            try
            {
                using (var sitecoreContext = new SitecoreContext())
                {
                    MW01DataSource model = new MW01DataSource();
                    var datasource = sitecoreContext.GetItem<MW01DataSource>(ItemPaths.ModalWindowSettings);

                    if (datasource != null)
                    {
                        model = datasource;

                        if (!string.IsNullOrWhiteSpace(model.Title))
                        {
                            model.Title = model.Title.Replace("'", "\\'");
                        }

                        if (!string.IsNullOrWhiteSpace(model.Text))
                        {
                            model.Text = model.Text.Replace("'", "\\'");
                        }
                    }

                    var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                    model.ClientId = authenticationDataSessionStorage.GetUserData().Identifier;
                    model.IsAccepted = isAccepted;

                    var generalSettings = ConfigHelpers.GetGeneralSettings();
                    ViewData["SelectAll_Text"] = Sitecore.Context.Item["SelectAll_Text"];
                    ViewData["IAmInformed"] = generalSettings.IAmInformed;
                    ViewData["IAgree"] = generalSettings.IAgree;
                    ViewData["Accept"] = generalSettings.Accept;

                    return View("/Areas/eContracting/Views/DocumentPanel.cshtml", model);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error when displaying document panel.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        /// <summary>
        /// Accepts offer.
        /// </summary>
        /// <returns>Instance result.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept()
        {
            try
            {
                var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                string guid = authenticationDataSessionStorage.GetUserData().Identifier;

                RweClient client = new RweClient();

                var documentsId = new List<string>();
                var offersNotsent = client.GetNotSentOffers();

                foreach (var offer in offersNotsent)
                {
                    documentsId.Add(offer.Guid);
                }

                // New acceptance logger
                if(documentsId.Count > 0)
                {
                    var service = new Rwe.Sc.AcceptanceLogger.Service.LoggerService();
                    service.LogAcceptance(guid, documentsId, "NABIDKA", DateTime.Now);
                }

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
