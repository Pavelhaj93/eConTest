using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient
{
    class MemorySettingsReaderService : ISettingsReaderService
    {
        readonly GlobalConfiguration Options;

        public MemorySettingsReaderService(GlobalConfiguration options)
        {
            this.Options = options;
        }

        public bool SaveFilesToDebugFolder { get; } = false;
        public bool ShowDebugMessages { get; } = true;
        public int SessionTimeout { get; } = 20;
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
            var list = new List<MemoryProcessModel>();
            list.Add(new MemoryProcessModel()
            {
                Title = "REFIX",
                Code = "00"
            });
            list.Add(new MemoryProcessModel()
            {
                Title = "RETENCE/LastCall",
                Code = "01"
            });
            list.Add(new MemoryProcessModel()
            {
                Title = "AKVIZICE",
                Code = "02"
            });
            list.Add(new MemoryProcessModel()
            {
                Title = "8003",
                Code = "03"
            });
            list.Add(new MemoryProcessModel()
            {
                Title = "BNR",
                Code = "04"
            });
            return list;
        }

        public IEnumerable<IProcessTypeModel> GetAllProcessTypes()
        {
            var list = new List<MemoryProcessTypeModel>();
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "Individuální",
                Code = "A"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "Kampaň",
                Code = "B"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "Sdružená BN",
                Code = "C"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "NC - Pojištění",
                Code = "D"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "NC - Senzor",
                Code = "E"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "NC - SPS",
                Code = "F"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "DSL - Investor",
                Code = "G"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "DSL - NSZ",
                Code = "H"
            });
            list.Add(new MemoryProcessTypeModel()
            {
                Title = "DSL - iKarta",
                Code = "I"
            });
            return list;
        }

        public CacheApiServiceOptions GetApiServiceOptions()
        {
            var options = new CacheApiServiceOptions(this.Options.ServiceUrl, this.Options.ServiceUser, this.Options.ServicePassword);
            return options;
        }

        public KeyValuePair<string, string>[] GetBackCompatibleTextParametersKeys(int version)
        {
            var list = new List<KeyValuePair<string, string>>();

            if (version == 1)
            {
                list.Add(new KeyValuePair<string, string>("CUSTTITLELET", "PERSON_CUSTTITLELET"));
                list.Add(new KeyValuePair<string, string>("CUSTADDRESS", "PERSON_PREMADR"));
                list.Add(new KeyValuePair<string, string>("PREMLABEL", "PERSON_PREMLABEL"));
                list.Add(new KeyValuePair<string, string>("PREMEXT", "PERSON_PREMTEXT"));
            }

            return list.ToArray();
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
            return this.GetDefinition(offer.Process, offer.ProcessType);
        }

        public IDefinitionCombinationModel GetDefinition(string process, string processType)
        {
            var model = new MemoryDefinitionCombinationModel()
            {
                Process = new MemoryProcessModel() { Code = process },
                ProcessType = new MemoryProcessTypeModel() { Code = processType }
            };
            model.LoginTypes = new List<MemoryLoginTypeModel>() { new MemoryLoginTypeModel() { Name = "dummy" } };
            model.MainTextLogin = new MemoryRichTextModel() { Text = "" };
            model.MainTextLoginAccepted = new MemoryRichTextModel() { Text = "" };
            model.MainTextThankYou = new MemoryRichTextModel() { Text = "" };
            model.OfferAcceptedMainText = new MemoryRichTextModel() { Text = "" };
            model.OfferAcceptText = new MemoryRichTextModel() { Text = "" };
            model.OfferAcceptTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferBenefitsTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferCommoditiesAcceptText = new MemoryRichTextModel() { Text = "" };
            model.OfferCommoditiesAcceptTitle = new MemorySimpleTextModel() { Text = "Accepted documents" };
            model.OfferCommoditiesSignText = new MemoryRichTextModel() { Text = "" };
            model.OfferCommoditiesSignTitle = new MemorySimpleTextModel() { Text = "Signed documents" };
            model.OfferCommoditiesText = new MemoryRichTextModel() { Text = "" };
            model.OfferCommoditiesTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferExpiredMainText = new MemoryRichTextModel() { Text = "" };
            model.OfferGiftsTitle = new MemorySimpleTextModel() { Text = "Gifts" };
            model.OfferMainText = new MemoryRichTextModel() { Text = "" };
            model.OfferAdditionalServicesDocsText = new MemoryRichTextModel() { Text = "" };
            model.OfferAdditionalServicesDocsTitle = new MemorySimpleTextModel() { Text = "Additional services" };
            model.OfferAdditionalServicesNote = new MemoryRichTextModel() { Text = "" };
            model.OfferAdditionalServicesSummaryTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferAdditionalServicesTitle = new MemorySimpleTextModel() { Text = "Additional services" };
            model.OfferOtherProductsDocsText = new MemoryRichTextModel() { Text = "" };
            model.OfferOtherProductsDocsTitle = new MemorySimpleTextModel() { Text = "Other products" };
            model.OfferOtherProductsNote = new MemoryRichTextModel() { Text = "" };
            model.OfferOtherProductsSummaryTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferOtherProductsTitle = new MemorySimpleTextModel() { Text = "Other products" };
            model.OfferPerexTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferTitle = new MemorySimpleTextModel() { Text = "" };
            model.OfferUploadsExtraHelp = new MemorySimpleTextModel() { Text = "" };
            model.OfferUploadsExtraText = new MemoryRichTextModel() { Text = "" };
            model.OfferUploadsNote = new MemoryRichTextModel() { Text = "" };
            model.OfferUploadsTitle = new MemorySimpleTextModel() { Text = "Upload" };
            return model;
            //return this.GetAllDefinitionCombinations().FirstOrDefault(x => x.Process.Code == process && x.ProcessType.Code == processType);
        }

        public IDefinitionCombinationModel GetDefinitionDefault()
        {
            return this.GetDefinition("default", "default");
        }

        public string GetFileCacheStorageConnectionString()
        {
            throw new NotImplementedException();
        }

        public ISiteSettingsModel GetGeneralSettings()
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
            var options = new SignApiServiceOptions(this.Options.ServiceSignUrl, this.Options.ServiceSignUser, this.Options.ServiceSignPassword);
            return options;
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
            var list = new List<string>();
            list.Add("BENEFITS_NOW_INTRO");
            list.Add("BENEFITS_NEXT_SIGN_INTRO");
            list.Add("BENEFITS_NEXT_TZD_INTRO");
            return list.ToArray();
        }

        protected IEnumerable<IDefinitionCombinationModel> GetAllDefinitionCombinations()
        {
            var list = new List<IDefinitionCombinationModel>();
            // 8003 Indi
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "8003");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // 8003 SBN
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "8003");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "C");
                list.Add(def);
            }
            // Akvizice indi
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "02");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // Akvizice kampan
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "02");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "B");
                list.Add(def);
            }
            // Akvizice kampan
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "02");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "C");
                list.Add(def);
            }
            // BNR ikarta
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "I");
                list.Add(def);
            }
            // BNR investor
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "G");
                list.Add(def);
            }
            // BNR NSZ
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "H");
                list.Add(def);
            }
            // BNR pojisteni
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "D");
                list.Add(def);
            }
            // BNR senzor
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "E");
                list.Add(def);
            }
            // BNR SPS
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "F");
                list.Add(def);
            }
            // Refix indi
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "00");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // Refix kampan
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "00");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "B");
                list.Add(def);
            }
            // Retence indi
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "01");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // Retence kampan
            {
                var def = new MemoryDefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "01");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "B");
                list.Add(def);
            }
            return list;
        }
    }
}
