using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eContracting.Kernel;
using eContracting.Kernel.GlassItems.Content.Modal_window;
using eContracting.Kernel.GlassItems.Settings;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
using Glass.Mapper.Sc;
using Sitecore.Mvc.Controllers;
using Log = Sitecore.Diagnostics.Log;

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
                    model.Item = Sitecore.Context.Database.GetItem(ItemPaths.ModalWindowSettings);

                    var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                    var authenticationDataItem = authenticationDataSessionStorage.GetUserData();

                    model.ClientId = authenticationDataItem.Identifier;
                    model.IsAccepted = isAccepted;
                    model.IsRetention = authenticationDataItem.OfferType == OfferTypes.Retention;
                    model.IsAcquisition = authenticationDataItem.OfferType == OfferTypes.Acquisition;

                    var generalSettings = ConfigHelpers.GetGeneralSettings();
                    ViewData["SelectAll_Text"] = Sitecore.Context.Item["SelectAll_Text"];
                    ViewData["IAmInformed"] = generalSettings.IAmInformed;
                    ViewData["IAgree"] = generalSettings.IAgree;
                    ViewData["Accept"] = generalSettings.Accept;

                    GeneralTextsSettings texts = null;

                    if (model.IsRetention)
                    {
                        texts = generalSettings.GetTexts(OfferTypes.Retention);
                    }
                    else if (model.IsAcquisition)
                    {
                        texts = generalSettings.GetTexts(OfferTypes.Acquisition);
                    }
                    else
                    {
                        texts = generalSettings.GetTexts(OfferTypes.Default);
                    }

                    if (texts != null)
                    {
                        ViewData["DocumentToSign"] = texts.DocumentToSign;
                        ViewData["DocumentToSignAccepted"] = texts.DocumentToSignAccepted;
                        ViewData["Step1Heading"] = texts.Step1Heading;
                        ViewData["Step2Heading"] = texts.Step2Heading;
                        ViewData["WhySignIsRequired"] = texts.WhySignIsRequired;
                        ViewData["SignButton"] = texts.SignButton;
                        ViewData["HowToSign"] = texts.HowToSign;
                        ViewData["HowToAccept"] = texts.HowToAccept;
                        ViewData["SignDocument"] = texts.SignDocument;
                        ViewData["SignRequest"] = texts.SignRequest;
                        ViewData["SignConfirm"] = texts.SignConfirm;
                        ViewData["SignDelete"] = texts.SignDelete;
                    }

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
            string guid = string.Empty;

            try
            {
                var ads = new AuthenticationDataSessionStorage();

                if (!ads.IsDataActive)
                {
                    return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SessionExpired).Url);
                }

                var data = ads.GetUserData();

                guid = data.Identifier;
                var offerType = data.OfferType;

                var client = new RweClient();

                var documentList = new List<string>();

                if (!string.IsNullOrEmpty(this.HttpContext.Request.Form["documents"]))
                {
                    documentList.AddRange(this.HttpContext.Request.Form["documents"].Split(','));
                }

                client.LogAcceptance(guid, DateTime.UtcNow, this.HttpContext, offerType, documentList);

                client.AcceptOffer(data.Identifier);
                data.IsAccepted = true;

                Log.Info($"[{guid}] Offer accepted", this);

                var redirectUrl = ConfigHelpers.GetPageLink(PageLinkType.ThankYou).Url;
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Log.Fatal($"[{guid}] Error when accepting offer.", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }
    }
}
