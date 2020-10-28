using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Services
{
    public class SitecoreSettingsReaderService : ISettingsReaderService
    {
        protected readonly ISitecoreContext Context;
        protected readonly static Random Rand = new Random();
        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreSettingsReaderService"/> class.
        /// </summary>
        public SitecoreSettingsReaderService(ISitecoreContext sitecoreContext)
        {
            this.Context = sitecoreContext ?? throw new ArgumentNullException(nameof(sitecoreContext));
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetAllLoginTypes()
        {
            var items = this.Context.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES);

            if (items?.Count() == 0)
            {
                throw new MissingDatasourceException("No login types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            var items =  this.Context.GetItems<ProcessModel>(Constants.SitecorePaths.PROCESSES);

            if (items?.Count() == 0)
            {
                throw new MissingDatasourceException("No processes found");
            }

            return items;
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            var items = this.Context.GetItems<ProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES);

            if (items?.Count() == 0)
            {
                throw new MissingDatasourceException("No process types found");
            }

            return items;
        }

        /// <inheritdoc/>
        public CacheApiServiceOptions GetApiServiceOptions()
        {
            var url = Sitecore.Configuration.Settings.GetSetting("eContracting.ServiceUrl");
            var username = Sitecore.Configuration.Settings.GetSetting("eContracting.ServiceUser");
            var password = Sitecore.Configuration.Settings.GetSetting("eContracting.ServicePassword");

            if (url == null || username == null || password == null)
            {
                var generalSettings = this.GetSiteSettings();
                url = url ?? generalSettings.ServiceUrl;
                username = username ?? generalSettings.ServiceUser;
                password = password ?? generalSettings.ServicePassword;
            }

            var options = new CacheApiServiceOptions(url, username, password);
            return options;
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            var definitions = this.Context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
            return definitions.FirstOrDefault(x => x.Process.Code == offer.Process && x.ProcessType.Code == offer.ProcessType);
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(string process, string processType)
        {
            var definitions = this.Context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
            return definitions.FirstOrDefault(x => x.Process.Code.Equals(process, StringComparison.InvariantCultureIgnoreCase) && x.ProcessType.Code.Equals(processType, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetLoginTypes(OfferModel offer)
        {
            var definition = this.GetDefinition(offer);
            var loginTypes = definition.LoginTypes.ToList();

            if (loginTypes.Count == 0)
            {
                var availableLoginTypes = this.GetAllLoginTypes().ToArray();
                var count = availableLoginTypes.Length;

                if (count > 1)
                {
                    var index = Rand.Next(0, count);
                    loginTypes.Add(availableLoginTypes[index]);
                }
                else
                {
                    loginTypes.AddRange(availableLoginTypes);
                }
            }

            return loginTypes;
        }

        /// <inheritdoc/>
        public RichTextModel GetMainTextForLogin(OfferModel offer)
        {
            var definition = this.GetDefinition(offer);
            return offer.IsAccepted ? definition.MainTextLoginAccepted : definition.MainTextLogin;
        }

        /// <inheritdoc/>
        public string GetPageLink(PageLinkTypes type)
        {
            var settings = this.GetSiteSettings();

            switch (type)
            {
                case PageLinkTypes.Offer:
                    return settings.Offer.Url;
                case PageLinkTypes.SessionExpired:
                    return settings.SessionExpired.Url;
                case PageLinkTypes.UserBlocked:
                    return settings.UserBlocked.Url;
                case PageLinkTypes.AcceptedOffer:
                    return settings.AcceptedOffer.Url;
                case PageLinkTypes.WrongUrl:
                    return settings.WrongUrl.Url;
                case PageLinkTypes.OfferExpired:
                    return settings.OfferExpired.Url;
                case PageLinkTypes.ThankYou:
                    return settings.ThankYou.Url;
                case PageLinkTypes.SystemError:
                    return settings.SystemError.Url;
                case PageLinkTypes.Welcome:
                    return settings.Welcome.Url;
                case PageLinkTypes.Login:
                    return settings.Login.Url;
                default:
                    throw new InvalidOperationException($"Invalid page type ({Enum.GetName(typeof(PageLinkTypes), type)}).");
            }
        }

        /// <inheritdoc/>
        public SiteSettingsModel GetSiteSettings()
        {
            var settings = this.Context.GetItem<SiteSettingsModel>(Sitecore.Context.Site.RootPath);
            return settings ?? throw new MissingDatasourceException("Site settings could not be resolved.");
        }

        /// <inheritdoc/>
        public ProcessStepModel[] GetSteps(ProcessStepModel currentStep)
        {
            var parentPath = currentStep.Path.Substring(0, currentStep.Path.LastIndexOf('/'));
            var items = this.Context.GetItems<ProcessStepModel>(parentPath);
            return items.ToArray();
        }
    }
}
