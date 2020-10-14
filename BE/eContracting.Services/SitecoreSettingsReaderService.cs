using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;
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
            return this.Context.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES);
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            return this.Context.GetItems<ProcessModel>(Constants.SitecorePaths.PROCESSES);
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            return this.Context.GetItems<ProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES);
        }

        /// <inheritdoc/>
        public CacheApiServiceOptions GetApiServiceOptions()
        {
            var generalSettings = this.GetSiteSettings();
            var options = new CacheApiServiceOptions(generalSettings.ServiceUser, generalSettings.ServicePassword, generalSettings.ServiceUrl);
            return options;
        }

        /// <inheritdoc/>
        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            var definitions = this.Context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
            return definitions.FirstOrDefault(x => x.Process.Code == offer.Process && x.ProcessType.Code == offer.ProcessType);
        }

        [Obsolete("Use 'GetSiteSettings' instead")]
        public SiteSettingsModel GetGeneralSettings()
        {
            throw new NotImplementedException();
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
                    var index = Rand.Next(0, count - 1);
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

        public string GetPageLink(PageLinkTypes type)
        {
            var settings = this.GetSiteSettings();

            switch (type)
            {
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
                    throw new InvalidOperationException("Invalid page type.");
            }
        }

        public SiteSettingsModel GetSiteSettings()
        {
            return this.Context.GetItem<SiteSettingsModel>(Sitecore.Context.Site.RootPath);
        }
    }
}
