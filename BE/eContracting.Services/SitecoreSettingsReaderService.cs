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
        /// <summary>
        /// Initializes a new instance of the <see cref="SitecoreSettingsReaderService"/> class.
        /// </summary>
        public SitecoreSettingsReaderService()
        {
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetAllLoginTypes()
        {
            using (var context = new SitecoreContext())
            {
                return context.GetItems<LoginTypeModel>(Constants.SitecorePaths.LOGIN_TYPES);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            using (var context = new SitecoreContext())
            {
                return context.GetItems<ProcessModel>(Constants.SitecorePaths.PROCESSES);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            using (var context = new SitecoreContext())
            {
                return context.GetItems<ProcessTypeModel>(Constants.SitecorePaths.PROCESS_TYPES);
            }
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
            using (var context = new SitecoreContext())
            {
                var definitions = context.GetItems<DefinitionCombinationModel>(Constants.SitecorePaths.DEFINITIONS);
                return definitions.FirstOrDefault(x => x.Process.Code == offer.Process && x.ProcessType.Code == offer.ProcessType);
            }
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
                var rand = new Random();
                var index = rand.Next(0, count - 1);
                loginTypes.Add(availableLoginTypes[index]);
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
            throw new NotImplementedException();
        }

        public SiteSettingsModel GetSiteSettings()
        {
            using (var context = new SitecoreContext())
            {
                return context.GetItem<SiteSettingsModel>(Sitecore.Context.Site.RootPath);
            }
        }
    }
}
