using System;
using System.Web.Mvc;
using eContracting.Kernel.GlassItems.Pages;
using eContracting.Kernel.Helpers;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;
using eContracting.Kernel.Utils;
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
            var guid = string.Empty;

            try
            {
                if (Sitecore.Context.PageMode.IsNormal)
                {
                    guid = Request.QueryString["guid"];

                    if (string.IsNullOrEmpty(guid))
                    {
                        return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                    }

                    if (this.CheckWhetherUserIsBlocked(guid))
                    {
                        return Redirect(ConfigHelpers.GetPageLink(PageLinkType.UserBlocked).Url);
                    }

                    var client = new RweClient();
                    var offer = client.GenerateXml(guid);

                    if ((offer == null) || (offer.OfferInternal.Body == null) || string.IsNullOrEmpty(offer.OfferInternal.Body.BIRTHDT))
                    {
                        return Redirect(ConfigHelpers.GetPageLink(PageLinkType.WrongUrl).Url);
                    }

                    if (offer.OfferInternal.State == "3")
                    {
                        client.ReadOffer(guid);
                    }

                    var authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
                    var authenticationData = authenticationDataSessionStorage.GetUserData(offer, true);

                    this.Context.OfferType = authenticationData.OfferType;
                    this.Context.IsIndividual = authenticationData.IsIndi;

                     var processingParameters = SystemHelpers.GetParameters(guid, authenticationData.OfferType, string.Empty);
                    this.HttpContext.Items["WelcomeData"] = processingParameters;

                    var dataModel = new WelcomeModel() { Guid = guid };
                    FillViewData(dataModel);

                    return View("/Areas/eContracting/Views/Welcome.cshtml", this.Context);
                }
                else
                {
                    //TODO: Remove after check
                    var dataModel = new WelcomeModel() { Guid = string.Empty };
                    FillViewData(dataModel);

                    return View("/Areas/eContracting/Views/Welcome.cshtml", this.Context);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[{guid}] Error when displaying welcome user page", ex, this);
                return Redirect(ConfigHelpers.GetPageLink(PageLinkType.SystemError).Url);
            }
        }

        [HttpPost]
        public ActionResult WelcomeSubmit(string guid)
        {
            return Redirect(ConfigHelpers.GetPageLink(PageLinkType.Login).Url + "?fromWelcome=1&guid=" + guid);
        }

        private void FillViewData(WelcomeModel dataModel)
        {
            dataModel.ButtonText = this.Context.ButtonText;
        }

    }
}
