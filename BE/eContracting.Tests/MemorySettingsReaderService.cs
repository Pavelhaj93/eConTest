using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    class MemorySettingsReaderService : ISettingsReaderService
    {
        public bool SaveFilesToDebugFolder { get; }
        public bool ShowDebugMessages { get; }
        public int SessionTimeout { get; }
        public int CognitoMinSecondsToRefreshToken { get; }

        public IDefinitionCombinationModel[] GetAllDefinitions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILoginTypeModel> GetAllLoginTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProcessModel> GetAllProcesses()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProcessTypeModel> GetAllProcessTypes()
        {
            throw new NotImplementedException();
        }

        public CacheApiServiceOptions GetApiServiceOptions()
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, string>[] GetBackCompatibleTextParametersKeys(int version)
        {
            throw new NotImplementedException();
        }

        public CognitoSettingsModel GetCognitoSettings()
        {
            throw new NotImplementedException();
        }

        public string GetCustomDatabaseConnectionString()
        {
            throw new NotImplementedException();
        }

        public IDefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public IDefinitionCombinationModel GetDefinition(string process, string processType)
        {
            throw new NotImplementedException();
        }

        public IDefinitionCombinationModel GetDefinitionDefault()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILoginTypeModel> GetLoginTypes(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public IRichTextModel GetMainTextForLogin(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public string GetPageLink(PAGE_LINK_TYPES type)
        {
            throw new NotImplementedException();
        }

        public string GetPageLink(PAGE_LINK_TYPES type, string guid)
        {
            throw new NotImplementedException();
        }

        public SignApiServiceOptions GetSignApiServiceOptions()
        {
            throw new NotImplementedException();
        }

        public ISiteSettingsModel GetSiteSettings()
        {
            throw new NotImplementedException();
        }

        public IProcessStepModel[] GetSteps(IProcessStepModel currentStep)
        {
            throw new NotImplementedException();
        }

        public string[] GetXmlNodeNamesExcludeHtml()
        {
            throw new NotImplementedException();
        }
    }
}
