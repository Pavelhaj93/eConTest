using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using eContracting.Services.Models;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient
{
    class MemorySettingsReaderService : ISettingsReaderService
    {
        readonly IOptions<GlobalConfiguration> Options;

        public MemorySettingsReaderService(IOptions<GlobalConfiguration> options)
        {
            this.Options = options;
        }
        public IEnumerable<LoginTypeModel> GetAllLoginTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            throw new NotImplementedException();
        }

        public CacheApiServiceOptions GetApiServiceOptions()
        {
            var options = new CacheApiServiceOptions(this.Options.Value.ServiceUser, this.Options.Value.ServicePassword, this.Options.Value.ServiceUrl);
            return options;
        }

        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public SiteSettingsModel GetGeneralSettings()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LoginTypeModel> GetLoginTypes(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public RichTextModel GetMainTextForLogin(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public string GetPageLink(PageLinkTypes type)
        {
            throw new NotImplementedException();
        }

        public SiteSettingsModel GetSiteSettings()
        {
            throw new NotImplementedException();
        }
    }
}
