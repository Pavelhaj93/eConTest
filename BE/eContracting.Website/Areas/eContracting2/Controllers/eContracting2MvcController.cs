using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using static eContracting.Website.Areas.eContracting2.Models.MatrixSwitcherViewModel;

namespace eContracting.Website.Areas.eContracting2.Controllers
{
    public abstract class eContracting2MvcController : Controller
    {
        protected readonly ILogger Logger;
        protected readonly IContextWrapper ContextWrapper;
        protected readonly IUserService UserService;
        protected readonly ISettingsReaderService SettingsService;
        protected readonly ISessionProvider SessionProvider;
        protected readonly IDataRequestCacheService RequestCacheService;

        protected eContracting2MvcController(
            ILogger logger,
            IContextWrapper contextWrapper,
            IUserService userService,
            ISettingsReaderService settingsReader,
            ISessionProvider sessionProvider,
            IDataRequestCacheService requestCacheService)
        {
            this.Logger = logger;
            this.ContextWrapper = contextWrapper;
            this.UserService = userService;
            this.SettingsService = settingsReader;
            this.SessionProvider = sessionProvider;
            this.RequestCacheService = requestCacheService;
        }

        public ActionResult MatrixSwitcher()
        {
            if (this.ContextWrapper.IsNormalMode())
            {
                return new EmptyResult();
            }

            var isPreview = this.ContextWrapper.IsPreviewMode();

            var url = this.Request.Url;
            url = Utils.RemoveQuery(url, Constants.QueryKeys.PROCESS);
            url = Utils.RemoveQuery(url, Constants.QueryKeys.PROCESS_TYPE);
            var query = HttpUtility.ParseQueryString(url.Query);

            var data = this.RequestCacheService.GetOffer(Constants.FakeOfferGuid);
            var definition = this.SettingsService.GetDefinition(data.Process, data.ProcessType);
            var defaultDefinition = this.SettingsService.GetDefinitionDefault();
            var allDefinitions = this.SettingsService.GetAllDefinitions();

            var defaultDefinitionViewModel = new DefinitionViewModel(defaultDefinition) { Name = "Výchozí" };
            var definitionsViewModels = new List<DefinitionViewModel>();
            definitionsViewModels.Add(defaultDefinitionViewModel);
            definitionsViewModels.AddRange(allDefinitions.Select(x => new DefinitionViewModel(x)));

            var viewModel = new MatrixSwitcherViewModel();
            viewModel.SubmitUrl = url.ToString();
            viewModel.Query = query;
            viewModel.CurrentDefinition = new DefinitionViewModel(definition);
            viewModel.Definitions = definitionsViewModels.ToArray();
            viewModel.IsPreview = isPreview;

            return View("/Areas/eContracting2/Views/Shared/MatrixSwitcher.cshtml", viewModel);
        }

        /// <summary>
        /// Gets guid value from query string.
        /// </summary>
        protected string GetGuid()
        {
            return this.Request.QueryString[Constants.QueryKeys.GUID];
        }

        protected RedirectResult Redirect(PAGE_LINK_TYPES pageType, string guid, string code = null, bool includeUtm = false)
        {
            var url = this.SettingsService.GetPageLink(pageType, guid);

            if (!string.IsNullOrEmpty(code))
            {
                url = Utils.SetQuery(url, Constants.QueryKeys.ERROR_CODE, code);
            }

            if (includeUtm)
            {
                var query = Utils.GetUtmQueryParams(this.ContextWrapper.GetQueryParams());
                url = Utils.SetQuery(url, query);
            }

            this.Logger.Debug(guid, "Redirecting to: " + url);
            return this.Redirect(url);
        }

        protected RedirectResult Redirect(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        protected string GetUrlWithGuid(PAGE_LINK_TYPES pageType, string guid)
        {
            var url = this.SettingsService.GetPageLink(pageType, guid);

            if (!string.IsNullOrEmpty(guid))
            {
                url = Utils.SetQuery(url, Constants.QueryKeys.GUID, guid);
            }

            return url;
        }

        protected bool CanRead(string guid)
        {
            return this.UserService.IsAuthorizedFor(guid);
        }

        protected void ClearCurrentUser(string guid)
        {
            this.UserService.Logout(guid);
            //this.UserService.Abandon(guid);
            // clear any user login, always establish a new session, when the user visits the login page - in order that he cannot slide among ThankYou, Offer and Login pages freely
            //this.SessionProvider.Abandon();
        }

        protected internal GoogleAnalyticsEvendDataModel GetGoogleEventData(
            OfferModel offer,
            string campaignLabel,
            string individualLabel,
            string electricityLabel,
            string gasLabel,
            string googleAnalyticsEcat,
            string eActDescription,
            string eLabDescription,
            IDefinitionCombinationModel definition)
        {
            var eCat = string.Empty;
            var eAct = string.Empty;
            var eLab = string.Empty;

            try
            {
                var type = offer.IsCampaign ? campaignLabel : individualLabel;
                var code = offer.IsCampaign ? offer.Campaign : offer.CreatedAt;
                var commodity = string.Empty;

                if (offer.Commodity.StartsWith(Constants.GTMElectricityIdentifier))
                {
                    commodity = electricityLabel;
                }
                else
                {
                    commodity = gasLabel;
                }

                var product = this.GetProductName(offer);

                var tokens = new Dictionary<string, string>();
                tokens.Add("TYPE", type);
                tokens.Add("CAMPAIGN", code);
                tokens.Add("COMMODITY", commodity);
                tokens.Add("PRODUCT", product);
                tokens.Add("BUS_PROCESS", definition.Process.Code);
                tokens.Add("BUS_PROCESS_NAME", definition.Process.e_Name);
                tokens.Add("BUS_TYPE", definition.ProcessType.Code);
                tokens.Add("BUS_TYPE_NAME", definition.ProcessType.e_Name);

                eCat = Utils.GetReplacedTextTokens(googleAnalyticsEcat, tokens);
                eAct = Utils.GetReplacedTextTokens(eActDescription, tokens);
                eLab = Utils.GetReplacedTextTokens(eLabDescription, tokens);
            }
            catch (Exception ex)
            {
                this.Logger.Error(offer.Guid, $"Cannot create data for Google Tag Manager (eCat, eAct, eLab)", ex);
            }

            return new GoogleAnalyticsEvendDataModel(eCat, eAct, eLab);
        }

        protected string GetProductName(OfferModel offer)
        {
            var product = "UNKNOWN";

            if (offer.TextParameters.ContainsKey("COMMODITY_PRODUCT") && !string.IsNullOrEmpty(offer.TextParameters["COMMODITY_PRODUCT"]))
            {
                product = offer.TextParameters["COMMODITY_PRODUCT"];
            }
            else if (offer.TextParameters.ContainsKey("NONCOMMODITY_PRODUCT") && !string.IsNullOrEmpty(offer.TextParameters["NONCOMMODITY_PRODUCT"]))
            {
                product = offer.TextParameters["NONCOMMODITY_PRODUCT"];
            }

            return product;
        }
    }
}
