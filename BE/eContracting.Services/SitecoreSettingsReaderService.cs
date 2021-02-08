using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        protected readonly ISitecoreContext Context;

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
        public bool SaveFilesToDebugFolder
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting("eContracting.SaveFilesToDebugFolder", false);
            }
        }

        /// <inheritdoc/>
        public bool ShowDebugMessages
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting("eContracting.ShowDebugMessages", false);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreSettingsReaderService"/> class.
        /// </summary>
        public SitecoreSettingsReaderService(ISitecoreContext sitecoreContext, IContextWrapper contextWrapper, ILogger logger)
        {
            this.Context = sitecoreContext ?? throw new ArgumentNullException(nameof(sitecoreContext));
            this.ContextWrapper = contextWrapper ?? throw new ArgumentNullException(nameof(contextWrapper));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetAllLoginTypes()
        {
            var items = this.Context.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES);

            if (!items.Any())
            {
                throw new EcontractingMissingDatasourceException("No login types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            var items =  this.Context.GetItems<ProcessModel>(Constants.SitecorePaths.PROCESSES);

            if (!items.Any())
            {
                throw new EcontractingMissingDatasourceException("No processes found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            var items = this.Context.GetItems<ProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES);

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
        public DefinitionCombinationModel GetDefinitionDefault()
        {
            var defaultDefinition = this.Context.GetItem<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);

            if (defaultDefinition == null)
            {
                throw new EcontractingMissingDatasourceException("Default definition combination not found. Cannot proceed with other execution without appropriate data.");
            }

            defaultDefinition.Process = new ProcessModel() { Code = "" };
            defaultDefinition.ProcessType = new ProcessTypeModel() { Code = "" };

            return defaultDefinition;
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            return this.GetDefinition(offer.Process, offer.ProcessType);
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(string process, string processType)
        {
            if (!string.IsNullOrEmpty(process) && !string.IsNullOrEmpty(processType))
            {
                var definitions = this.Context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
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
        public IEnumerable<LoginTypeModel> GetLoginTypes(OfferModel offer)
        {
            var list = new List<LoginTypeModel>();
            var definition = this.GetDefinition(offer);
            var availableLoginTypes = definition.LoginTypes?.ToArray() ?? new LoginTypeModel[] { };

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
        public RichTextModel GetMainTextForLogin(OfferModel offer)
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
                default:
                    throw new InvalidOperationException($"Invalid page type ({Enum.GetName(typeof(PAGE_LINK_TYPES), type)}).");
            }
        }

        /// <inheritdoc/>
        public SiteSettingsModel GetSiteSettings()
        {
            //TODO: sometimes this.Context.Database = null when calling it from api controller.
            var settings = this.Context.GetItem<SiteSettingsModel>(this.ContextWrapper.GetSiteRoot());
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
        public ProcessStepModel[] GetSteps(ProcessStepModel currentStep)
        {
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));
            var items = this.Context.GetItems<ProcessStepModel>(parentPath);

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
                list.Add(new KeyValuePair<string, string>("CUSTADDRESS" , "PERSON_CUSTADDRESS"));
                list.Add(new KeyValuePair<string, string>("PREMADR"     , "PERSON_PREMADR"));
                list.Add(new KeyValuePair<string, string>("PREMLABEL"   , "PERSON_PREMLABEL"));
                list.Add(new KeyValuePair<string, string>("PREMEXT"     , "PERSON_PREMEXT"));
            }

            return list.ToArray();
        }
    }
}
