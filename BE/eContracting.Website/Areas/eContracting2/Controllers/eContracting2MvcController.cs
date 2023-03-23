using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eContracting.Models;
using eContracting.Website.Areas.eContracting2.Models;
using Glass.Mapper.Sc.Web.Mvc;
using Sitecore.Data;
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
        protected readonly IMvcContext MvcContext;

        protected eContracting2MvcController(
            ILogger logger,
            IContextWrapper contextWrapper,
            IUserService userService,
            ISettingsReaderService settingsReader,
            ISessionProvider sessionProvider,
            IDataRequestCacheService requestCacheService,
            IMvcContext mvcContext)
        {
            this.Logger = logger;
            this.ContextWrapper = contextWrapper;
            this.UserService = userService;
            this.SettingsService = settingsReader;
            this.SessionProvider = sessionProvider;
            this.RequestCacheService = requestCacheService;
            this.MvcContext = mvcContext;
        }

        [ExcludeFromCodeCoverage]
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
        protected internal string GetGuid()
        {
            return this.Request.QueryString[Constants.QueryKeys.GUID];
        }

        protected internal RedirectResult Redirect(PAGE_LINK_TYPES pageType, string guid, string code = null, bool includeUtm = false)
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

        protected internal RedirectResult RedirectWithNewSession(PAGE_LINK_TYPES pageType, string guid, bool includeUtm = false)
        {
            var redirectUrl = this.SettingsService.GetPageLink(pageType, guid);
            
            if (includeUtm)
            {
                var query = Utils.GetUtmQueryParams(this.ContextWrapper.GetQueryParams());
                redirectUrl = Utils.SetQuery(redirectUrl, query);
            }

            var url = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.Login, guid);
            url = Utils.SetQuery(url, Constants.QueryKeys.RENEW_SESSION, "0");
            url = Utils.SetQuery(url, Constants.QueryKeys.REDIRECT, redirectUrl);
            this.Logger.Debug(guid, "Redirecting to: " + url);
            return this.Redirect(url);
        }

        protected internal bool CanRead(string guid)
        {
            return this.UserService.IsAuthorizedFor(guid);
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

        protected internal string GetProductName(OfferModel offer)
        {
            var product = "UNKNOWN";

            if (offer.TextParameters.HasValue("COMMODITY_PRODUCT"))
            {
                product = offer.TextParameters["COMMODITY_PRODUCT"];
            }
            else if (offer.TextParameters.HasValue("NONCOMMODITY_PRODUCT"))
            {
                product = offer.TextParameters["NONCOMMODITY_PRODUCT"];
            }

            return product;
        }

        protected internal string GetLogoutUrl(AUTH_METHODS authType, string logoutGuid, string returnGuid = null)
        {
            string redirectUrl = string.Empty;

            if (string.IsNullOrEmpty(returnGuid))
            {
                returnGuid = logoutGuid;
            }

            if (authType == AUTH_METHODS.TWO_SECRETS)
            {
                var url = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.Login, returnGuid);
                redirectUrl = $"{this.Request.Url.Scheme}://{this.Request.Url.Host}/logout?{Constants.QueryKeys.REDIRECT}=" + HttpUtility.UrlEncode(url);
                redirectUrl = Utils.SetQuery(redirectUrl, Constants.QueryKeys.GUID, logoutGuid);
            }
            else
            {
                var logout = this.SettingsService.GetPageLink(PAGE_LINK_TYPES.Logout, logoutGuid);
                logout = Utils.SetQuery(logout, Constants.QueryKeys.GUID, returnGuid);
                var url = this.SettingsService.GetCognitoSettings().InnogyLogoutUrl;
                url = Utils.SetQuery(url, Constants.QueryKeys.GLOBAL_LOGOUT, "true");
                redirectUrl = Utils.SetQuery(url, Constants.QueryKeys.REDIRECT, logout);
            }

            return redirectUrl;
        }

        protected internal IStepsModel GetSteps(OfferCacheDataModel offer, IBasePageWithStepsModel page)
        {
            return this.GetSteps(new UserCacheDataModel(), (OfferModel)null, page, null);
        }

        protected internal IStepsModel GetSteps(UserCacheDataModel user, OfferModel offer, IBasePageModel page, IDefinitionCombinationModel definition)
        {
            IStepsModel steps = null;

            if (offer.Version < 3)
            {
                steps = definition.StepsDefault;
            }
            else
            {
                steps = definition.Steps;
            }

            if (steps == null)
            {
                this.Logger.Debug(offer.Guid, "No steps defined");
                return null;
            }

            if (!steps.Steps.Any())
            {
                this.Logger.Debug(offer.Guid, $"Steps definition '{steps.Path}' does not have any child");
                return null;
            }

            this.Logger.Debug(offer.Guid, $"Steps definition '{steps.Path}' used");

            foreach (var step in steps.Steps)
            {
                if (step.TargetPage != null && step.TargetPage.ID == page.ID)
                {
                    step.IsSelected = true;
                    this.Logger.Debug(offer.Guid, $"Step '{step.Path}' marked as selected");
                }
            }

            if (!steps.AnySelected())
            {
                return null;
            }

            return steps;
        }
    }
}
