using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using eContracting.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data.Fields;
using Sitecore.Reflection.Emit;

namespace eContracting.Services
{
    public class SitecoreSettingsReaderService : ISettingsReaderService
    {
        /// <summary>
        /// The context.
        /// </summary>
        protected readonly ISitecoreService SitecoreService;

        /// <summary>
        /// The context wrapper.
        /// </summary>
        protected readonly IContextWrapper ContextWrapper;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The randomizer.
        /// </summary>
        protected readonly static Random Rand = new Random();

        /// <inheritdoc/>
        /// <remarks>Default value is false.</remarks>
        public bool SaveFilesToDebugFolder
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting("eContracting.SaveFilesToDebugFolder", false);
            }
        }

        /// <inheritdoc/>
        /// <remarks>Default value is false.</remarks>
        public bool ShowDebugMessages
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting("eContracting.ShowDebugMessages", false);
            }
        }

        /// <inheritdoc/>
        /// <remarks>Default value is 30 minutes.</remarks>
        public int SessionTimeout
        {
            get
            {
                return Sitecore.Configuration.Settings.GetIntSetting("eContracting.SessionTimeout", 30);
            }
        }

        /// <inheritdoc/>
        /// <remarks>Default value is 60 minutes.</remarks>
        public int CognitoMinSecondsToRefreshToken
        {
            get
            {
                return Sitecore.Configuration.Settings.GetIntSetting("eContracting.Cognito.MinimumSecondsToRefreshToken", 180);
            }
        }

        /// <inheritdoc/>
        /// <remarks>Default value is 5 seconds.</remarks>
        public int SubmitOfferDelay
        {
            get
            {
                return Sitecore.Configuration.Settings.GetIntSetting("eContracting.SubmitOfferDelay", 5);
            }
        }

        /// <inheritdoc/>
        /// <remarks>Default value is 5 seconds.</remarks>
        public int CancelOfferDelay
        {
            get
            {
                return Sitecore.Configuration.Settings.GetIntSetting("eContracting.CancelOfferDelay", 5);
            }
        }

        /// <inheritdoc/>
        public Uri CrmUtilitiesUmc
        {
            get
            {
                var endpoint = Sitecore.Configuration.Settings.GetSetting("eContracting.CRM_UTILITIES_UMC");

                if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
                {
                    throw new EcontractingApplicationException("Setting 'eContracting.CRM_UTILITIES_UMC' is not well formed url");
                }

                return new Uri(endpoint);
            }
        }

        /// <inheritdoc/>
        public Uri CrmAuthService
        {
            get
            {
                var endpoint = Sitecore.Configuration.Settings.GetSetting("eContracting.ZCRM_AUTH_SRV");

                if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
                {
                    throw new EcontractingApplicationException("Setting 'eContracting.ZCRM_AUTH_SRV' is not well formed url");
                }

                return new Uri(endpoint);
            }
        }

        /// <inheritdoc/>
        public string SapApiGatewayId
        {
            get
            {
                const string key = "ApiGateway.Api.Id";
                string value = null;

                if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    value = WebConfigurationManager.AppSettings[key];
                }
                else
                {
                    value = Sitecore.Configuration.Settings.GetSetting($"eContracting.{key}");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' doesn't exist");
                }

                return value;
            }
        }

        public Uri SapApiGatewayUrl
        {
            get
            {
                const string key = "CrmUmc.UrlApiGateway";
                string value = null;

                if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    value = WebConfigurationManager.AppSettings[key];
                }
                else
                {
                    value = Sitecore.Configuration.Settings.GetSetting($"eContracting.{key}");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' doesn't exist");
                }

                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' is not well formed url");
                }

                return new Uri(value);
            }
        }

        /// <inheritdoc/>
        public Uri CrmCognitoUrl
        {
            get
            {
                const string key = "CrmUmc.UrlApiGateway";
                string value = null;

                if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    value = WebConfigurationManager.AppSettings[key];
                }
                else
                {
                    value = Sitecore.Configuration.Settings.GetSetting($"eContracting.{key}");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' doesn't exist");
                }

                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' is not well formed url");
                }

                return new Uri(value);
            }
        }

        /// <inheritdoc/>
        public Uri CrmAnonymousUrl
        {
            get
            {
                const string key = "CrmUmcAnonymUser.Url";
                string value = null;

                if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    value = WebConfigurationManager.AppSettings[key];
                }
                else
                {
                    value = Sitecore.Configuration.Settings.GetSetting($"eContracting.{key}");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' doesn't exist");
                }

                if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' is not well formed url");
                }

                return new Uri(value);
            }
        }

        /// <inheritdoc/>
        public string CrmAnonymousUser
        {
            get
            {
                const string key = "CrmUmcAnonymUser.User";
                string value = null;

                if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                   value = WebConfigurationManager.AppSettings[key];
                }
                else
                {
                    value = Sitecore.Configuration.Settings.GetSetting($"eContracting.{key}");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' doesn't exist");
                }

                return value;
            }
        }

        /// <inheritdoc/>
        public string CrmAnonymousPassword
        {
            get
            {
                const string key = "CrmUmcAnonymUser.Pass";
                string value = null;

                if (WebConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    value = WebConfigurationManager.AppSettings[key];
                }
                else
                {
                    value = Sitecore.Configuration.Settings.GetSetting($"eContracting.{key}");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new EcontractingApplicationException($"Configuration for AppSettings['{key}'] nor Sitecore setting 'eContracting.{key}' doesn't exist");
                }

                return value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreSettingsReaderService"/> class.
        /// </summary>
        public SitecoreSettingsReaderService(ISitecoreService sitecoreService, IContextWrapper contextWrapper, ILogger logger)
        {
            this.SitecoreService = sitecoreService ?? throw new ArgumentNullException(nameof(sitecoreService));
            this.ContextWrapper = contextWrapper ?? throw new ArgumentNullException(nameof(contextWrapper));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public IEnumerable<ILoginTypeModel> GetAllLoginTypes()
        {
            var items = this.SitecoreService.GetItems<ILoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES);

            if (!items.Any())
            {
                throw new EcontractingMissingDatasourceException("No login types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<IProcessModel> GetAllProcesses()
        {
            var items = this.SitecoreService.GetItems<IProcessModel>(Constants.SitecorePaths.PROCESSES);

            if (!items.Any())
            {
                throw new EcontractingMissingDatasourceException("No processes found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<IProcessTypeModel> GetAllProcessTypes()
        {
            var items = this.SitecoreService.GetItems<IProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES);

            if (!items.Any())
            {
                throw new EcontractingMissingDatasourceException("No process types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public CacheApiServiceOptions GetApiServiceOptions()
        {
            var generalSettings = this.GetSiteSettings();

            var url = generalSettings.ServiceUrl;
            var username = generalSettings.ServiceUser;
            var password = generalSettings.ServicePassword;

            if (string.IsNullOrWhiteSpace(url))
            {
                url = this.ContextWrapper.GetSetting("eContracting.ServiceUrl");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                username = this.ContextWrapper.GetSetting("eContracting.ServiceUser");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = this.ContextWrapper.GetSetting("eContracting.ServicePassword");
            }

            var options = new CacheApiServiceOptions(url, username, password);
            return options;
        }

        /// <inheritdoc/>
        public SignApiServiceOptions GetSignApiServiceOptions()
        {
            var generalSettings = this.GetSiteSettings();

            var url = generalSettings.SigningServiceUrl;
            var username = generalSettings.SigningServiceUser;
            var password = generalSettings.SigningServicePassword;

            if (string.IsNullOrWhiteSpace(url))
            {
                url = this.ContextWrapper.GetSetting("eContracting.SigningServiceUrl");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                username = this.ContextWrapper.GetSetting("eContracting.SigningServiceUser");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                password = this.ContextWrapper.GetSetting("eContracting.SigningServicePassword");
            }

            var options = new SignApiServiceOptions(url, username, password);
            return options;
        }

        /// <inheritdoc/>
        public IDefinitionCombinationModel GetDefinitionDefault()
        {
            var defaultDefinition = this.SitecoreService.GetItem<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);

            if (defaultDefinition == null)
            {
                throw new EcontractingMissingDatasourceException("Default definition combination not found. Cannot proceed with other execution without appropriate data.");
            }

            defaultDefinition.Process = new ProcessModel() { Code = "" };
            defaultDefinition.ProcessType = new ProcessTypeModel() { Code = "" };

            return defaultDefinition;
        }

        /// <inheritdoc/>
        public IDefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            if (offer == null)
            {
                throw new EcontractingDataException(new ErrorModel("SETTING-GET-DEF", "Offer is null, cannot retrieve matrix information."));
            }

            if (!string.IsNullOrEmpty(offer.Process) && !string.IsNullOrEmpty(offer.ProcessType))
            {
                var allDefinitions = this.GetAllDefinitions();
                var definitions = allDefinitions.Where(x => x.Process.Code.Equals(offer.Process, StringComparison.InvariantCultureIgnoreCase) && x.ProcessType.Code.Equals(offer.ProcessType, StringComparison.InvariantCultureIgnoreCase));

                if (definitions.Any())
                {
                    var count = definitions.Count();

                    this.Logger.Debug(offer.Guid, $"{count} matrix definitions found for process '{offer.Process}' and process type '{offer.ProcessType}'");
                    
                    if (count == 1)
                    {
                        return definitions.First();
                    }

                    var offerAttributes = new NameValueCollection();

                    foreach (var attr in offer.Attributes)
                    {
                        offerAttributes.Add(attr.Key, attr.Value);
                    }

                    var orderedDefinitions = definitions.OrderByDescending(x => x.GetXmlAttributes().Count).ToArray();

                    var logBuilder = new StringBuilder();
                    logBuilder.AppendLine($"Looking for matrix definition (more definitions found) matching for:");
                    logBuilder.AppendLine($" - process = {offer.Process}");
                    logBuilder.AppendLine($" - process type = {offer.ProcessType}");
                    logBuilder.AppendLine("Matrix definitions found (ordered by attributes count):");

                    for (int i = 0; i < orderedDefinitions.Length; i++)
                    {
                        logBuilder.AppendLine($" - {orderedDefinitions[i].Path}");
                    }

                    for (int i = 0; i < orderedDefinitions.Length; i++)
                    {
                        var definition = orderedDefinitions[i];
                        var definitionAttributes = definition.GetXmlAttributes();

                        if (offerAttributes.Contains(definitionAttributes))
                        {
                            this.Logger.Debug(offer.Guid, $"Matrix definition found ({definition.Path}) for '{offer.Process}', process type '{offer.ProcessType}' and attributes {Utils.GetQueryString(definitionAttributes)}");
                            return definition;
                        }
                    }

                    var lastDefinition = orderedDefinitions.Last(); // Because matrix definitions were ordered by attributes count, last is taken
                    this.Logger.Debug(offer.Guid, $"No matrix definition matched to attributes. Taking last in row ({lastDefinition.Path})");
                    return lastDefinition;
                }
            }
            else
            {
                this.Logger.Warn(offer.Guid, $"Cannot determinate matrix definition because process and process type are missing");
            }

            this.Logger.Warn(null, $"Matrix definition not found for process '{offer.Process}' and process type '{offer.ProcessType}'. Taking default one..");

            return this.GetDefinitionDefault();
        }

        /// <inheritdoc/>
        public IDefinitionCombinationModel GetDefinition(OffersModel offer)
        {
            if (offer == null)
            {
                throw new EcontractingDataException(new ErrorModel("SETTING-GET-DEF-2", "Offers container is null, cannot retrieve matrix information."));
            }

            var firstOffer = offer.FirstOrDefault();

            if (firstOffer == null)
            {
                throw new EcontractingDataException(new ErrorModel("SETTING-GET-DEF", "Offers container doesn't contain any offer, cannot retrieve matrix information."));
            }

            return this.GetDefinition(firstOffer);
        }

        /// <inheritdoc/>
        public IDefinitionCombinationModel GetDefinition(string process, string processType)
        {
            if (!string.IsNullOrEmpty(process) && !string.IsNullOrEmpty(processType))
            {
                var definitions = this.GetAllDefinitions();
                var definition = definitions.FirstOrDefault(x => x.Process.Code.Equals(process, StringComparison.InvariantCultureIgnoreCase) && x.ProcessType.Code.Equals(processType, StringComparison.InvariantCultureIgnoreCase));

                if (definition != null)
                {
                    return definition;
                }
            }

            this.Logger.Warn(null, $"Definition combination not found for process '{process}' and process type '{processType}'. Taking default one..");

            return this.GetDefinitionDefault();
        }

        /// <inheritdoc/>
        public IDefinitionCombinationModel[] GetAllDefinitions()
        {
            return this.SitecoreService.GetItems<IDefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS).ToArray();
        }

        /// <inheritdoc/>
        public IProductInfoModel GetProductInfo(OfferAttachmentModel attachmentModel)
        {
            if (attachmentModel == null)
            {
                return null;
            }

            var definitions = this.SitecoreService.GetItems<IProductInfoModel>(Constants.SitecorePaths.PRODUCT_INFOS).ToArray();
            var model = definitions.FirstOrDefault(x => x.Key == attachmentModel.Product);
            return model;
        }

        /// <inheritdoc/>
        public IProductInfoModel[] GetAllProductInfos()
        {
            var definitions = this.SitecoreService.GetItems<IProductInfoModel>(Constants.SitecorePaths.PRODUCT_INFOS).ToArray();
            return definitions;
        }

        /// <inheritdoc/>
        public IEnumerable<ILoginTypeModel> GetLoginTypes(OffersModel offer)
        {
            var list = new List<ILoginTypeModel>();
            var definition = this.GetDefinition(offer);
            var availableLoginTypes = definition.LoginTypes?.ToArray() ?? new ILoginTypeModel[] { };

            // backup option when content configuration is wrong
            if (availableLoginTypes.Length == 0)
            {
                availableLoginTypes = this.GetAllLoginTypes().ToArray();
            }

            if (definition.LoginTypesRandom && availableLoginTypes.Length > 1)
            {
                var count = availableLoginTypes.Length;
                var index = Rand.Next(0, count);
                list.Add(availableLoginTypes[index]);
            }
            else
            {
                list.AddRange(availableLoginTypes);
            }

            return list;
        }

        /// <inheritdoc/>
        public IRichTextModel GetMainTextForLogin(OffersModel offer)
        {
            var definition = this.GetDefinition(offer);
            return offer.IsAccepted ? definition.MainTextLoginAccepted : definition.MainTextLogin;
        }

        /// <inheritdoc/>
        public string GetPageLink(PAGE_LINK_TYPES type)
        {
            var settings = this.GetSiteSettings();

            switch (type)
            {
                case PAGE_LINK_TYPES.Summary:
                    return settings.Summary.Url;
                case PAGE_LINK_TYPES.Offer:
                    return settings.Offer.Url;
                case PAGE_LINK_TYPES.SessionExpired:
                    return settings.SessionExpired.Url;
                case PAGE_LINK_TYPES.UserBlocked:
                    return settings.UserBlocked.Url;
                case PAGE_LINK_TYPES.AcceptedOffer:
                    return settings.AcceptedOffer.Url;
                case PAGE_LINK_TYPES.WrongUrl:
                    return settings.WrongUrl.Url;
                case PAGE_LINK_TYPES.OfferExpired:
                    return settings.ExpiredOffer.Url;
                case PAGE_LINK_TYPES.ThankYou:
                    return settings.ThankYou.Url;
                case PAGE_LINK_TYPES.SystemError:
                    return settings.SystemError.Url;
                case PAGE_LINK_TYPES.Login:
                    return settings.Login.Url;
                case PAGE_LINK_TYPES.Logout:
                    {
                        var builder = new UriBuilder(settings.Login.Url);
                        builder.Path = "logout";
                        return builder.Uri.ToString();
                    }
                default:
                    throw new InvalidOperationException($"Invalid page type ({Enum.GetName(typeof(PAGE_LINK_TYPES), type)}).");
            }
        }

        public string GetPageLink(PAGE_LINK_TYPES type, string guid)
        {
            var url = this.GetPageLink(type);
            url = Utils.SetQuery(url, Constants.QueryKeys.GUID, guid ?? string.Empty);
            return url;
        }

        /// <inheritdoc/>
        public ISiteSettingsModel GetSiteSettings()
        {
            //TODO: sometimes this.Context.Database = null when calling it from api controller.
            var settings = this.SitecoreService.GetItem<ISiteSettingsModel>(this.ContextWrapper.GetSiteRoot());
            return settings ?? throw new EcontractingMissingDatasourceException("Site settings could not be resolved.");
        }

        /// <inheritdoc/>
        public string[] GetXmlNodeNamesExcludeHtml()
        {
            var list = new List<string>();
            list.Add("BENEFITS_NOW_INTRO");
            list.Add("BENEFITS_NEXT_SIGN_INTRO");
            list.Add("BENEFITS_NEXT_TZD_INTRO");
            return list.ToArray();
        }

        /// <inheritdoc/>
        public IStepModel[] GetSteps(IStepModel currentStep)
        {
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));
            var items = this.SitecoreService.GetItems<IStepModel>(parentPath);

            foreach (var item in items)
            {
                if (item.ID == currentStep.ID)
                {
                    item.IsSelected = true;
                }
            }

            return items.ToArray();
        }

        /// <inheritdoc/>
        public string GetCustomDatabaseConnectionString()
        {
            string name = Constants.DatabaseContextConnectionStringName;

            if (string.IsNullOrEmpty(name))
            {
                throw new ApplicationException("Name of connection string is empty");
            }

            var conString = System.Configuration.ConfigurationManager.ConnectionStrings[name];

            if (conString == null)
            {
                throw new ApplicationException($"Connection string '{name}' doesn't exist");
            }

            return conString.ConnectionString;
        }

        /// <inheritdoc/>
        public KeyValuePair<string, string>[] GetBackCompatibleTextParametersKeys(int version)
        {
            var list = new List<KeyValuePair<string, string>>();

            if (version == 1)
            {
                list.Add(new KeyValuePair<string, string>("CUSTTITLELET", "PERSON_CUSTTITLELET"));
                list.Add(new KeyValuePair<string, string>("CUSTADDRESS", "PERSON_CUSTADDRESS"));
                list.Add(new KeyValuePair<string, string>("PREMADR", "PERSON_PREMADR"));
                list.Add(new KeyValuePair<string, string>("PREMLABEL", "PERSON_PREMLABEL"));
                list.Add(new KeyValuePair<string, string>("PREMEXT", "PERSON_PREMEXT"));
            }

            return list.ToArray();
        }

        /// <inheritdoc/>
        public CognitoSettingsModel GetCognitoSettings()
        {
            var cognitoBaseUrl = Sitecore.Configuration.Settings.GetSetting("eContracting.Cognito.OAuth.BaseUrl");
            var cognitoTokensUrl = Sitecore.Configuration.Settings.GetSetting("eContracting.Cognito.OAuth.TokensUrl");
            var cognitoClientId = Sitecore.Configuration.Settings.GetSetting("eContracting.Cognito.ClientId");
            var cognitoCookiePrefix = Sitecore.Configuration.Settings.GetSetting("eContracting.Cognito.CookiePrefix", "CognitoIdentityServiceProvider");
            var cognitoCookieUser = Sitecore.Configuration.Settings.GetSetting("eContracting.Cognito.CookieUser", "LastAuthUser");
            var innogyLoginUrl = Sitecore.Configuration.Settings.GetSetting("eContracting.Innogy.LoginUrl");
            var innogyLogoutUrl = Sitecore.Configuration.Settings.GetSetting("eContracting.Innogy.LogoutUrl");
            var innogyRegistrationUrl = Sitecore.Configuration.Settings.GetSetting("eContracting.Innogy.RegistrationUrl");
            var innogyDashboardUrl = Sitecore.Configuration.Settings.GetSetting("eContracting.Innogy.DashboardUrl");

            return new CognitoSettingsModel(cognitoBaseUrl, cognitoTokensUrl, cognitoClientId, cognitoCookiePrefix, cognitoCookieUser, innogyLoginUrl, innogyLogoutUrl, innogyRegistrationUrl, innogyDashboardUrl);
        }
    }
}
