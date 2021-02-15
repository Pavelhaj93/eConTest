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

        public IEnumerable<LoginTypeModel> GetAllLoginTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProcessModel> GetAllProcesses()
        {
            var list = new List<ProcessModel>();
            list.Add(new ProcessModel()
            {
                Title = "REFIX",
                Code = "00"
            });
            list.Add(new ProcessModel()
            {
                Title = "RETENCE/LastCall",
                Code = "01"
            });
            list.Add(new ProcessModel()
            {
                Title = "AKVIZICE",
                Code = "02"
            });
            list.Add(new ProcessModel()
            {
                Title = "8003",
                Code = "03"
            });
            list.Add(new ProcessModel()
            {
                Title = "BNR",
                Code = "04"
            });
            return list;
        }

        public IEnumerable<ProcessTypeModel> GetAllProcessTypes()
        {
            var list = new List<ProcessTypeModel>();
            list.Add(new ProcessTypeModel()
            {
                Title = "Individuální",
                Code = "A"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "Kampaň",
                Code = "B"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "Sdružená BN",
                Code = "C"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "NC - Pojištění",
                Code = "D"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "NC - Senzor",
                Code = "E"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "NC - SPS",
                Code = "F"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "DSL - Investor",
                Code = "G"
            });
            list.Add(new ProcessTypeModel()
            {
                Title = "DSL - NSZ",
                Code = "H"
            });
            list.Add(new ProcessTypeModel()
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

        public string GetCustomDatabaseConnectionString()
        {
            throw new NotImplementedException();
        }

        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            return this.GetDefinition(offer.Process, offer.ProcessType);
        }

        public DefinitionCombinationModel GetDefinition(string process, string processType)
        {
            var model = new DefinitionCombinationModel()
            {
                Process = new ProcessModel() { Code = process },
                ProcessType = new ProcessTypeModel() { Code = processType }
            };
            model.LoginTypes = new List<LoginTypeModel>() { new LoginTypeModel() { Name = "dummy" } };
            model.MainTextLogin = new RichTextModel() { Text = "" };
            model.MainTextLoginAccepted = new RichTextModel() { Text = "" };
            model.MainTextThankYou = new RichTextModel() { Text = "" };
            model.OfferAcceptedMainText = new RichTextModel() { Text = "" };
            model.OfferAcceptText = new RichTextModel() { Text = "" };
            model.OfferAcceptTitle = new SimpleTextModel() { Text = "" };
            model.OfferBenefitsTitle = new SimpleTextModel() { Text = "" };
            model.OfferCommoditiesAcceptText = new RichTextModel() { Text = "" };
            model.OfferCommoditiesAcceptTitle = new SimpleTextModel() { Text = "Accepted documents" };
            model.OfferCommoditiesSignText = new RichTextModel() { Text = "" };
            model.OfferCommoditiesSignTitle = new SimpleTextModel() { Text = "Signed documents" };
            model.OfferCommoditiesText = new RichTextModel() { Text = "" };
            model.OfferCommoditiesTitle = new SimpleTextModel() { Text = "" };
            model.OfferExpiredMainText = new RichTextModel() { Text = "" };
            model.OfferGiftsTitle = new SimpleTextModel() { Text = "Gifts" };
            model.OfferMainText = new RichTextModel() { Text = "" };
            model.OfferAdditionalServicesDocsText = new RichTextModel() { Text = "" };
            model.OfferAdditionalServicesDocsTitle = new SimpleTextModel() { Text = "Additional services" };
            model.OfferAdditionalServicesNote = new RichTextModel() { Text = "" };
            model.OfferAdditionalServicesSummaryTitle = new SimpleTextModel() { Text = "" };
            model.OfferAdditionalServicesTitle = new SimpleTextModel() { Text = "Additional services" };
            model.OfferOtherProductsDocsText = new RichTextModel() { Text = "" };
            model.OfferOtherProductsDocsTitle = new SimpleTextModel() { Text = "Other products" };
            model.OfferOtherProductsNote = new RichTextModel() { Text = "" };
            model.OfferOtherProductsSummaryTitle = new SimpleTextModel() { Text = "" };
            model.OfferOtherProductsTitle = new SimpleTextModel() { Text = "Other products" };
            model.OfferPerexTitle = new SimpleTextModel() { Text = "" };
            model.OfferTitle = new SimpleTextModel() { Text = "" };
            model.OfferUploadsExtraHelp = new SimpleTextModel() { Text = "" };
            model.OfferUploadsExtraText = new RichTextModel() { Text = "" };
            model.OfferUploadsNote = new RichTextModel() { Text = "" };
            model.OfferUploadsTitle = new SimpleTextModel() { Text = "Upload" };
            return model;
            //return this.GetAllDefinitionCombinations().FirstOrDefault(x => x.Process.Code == process && x.ProcessType.Code == processType);
        }

        public DefinitionCombinationModel GetDefinitionDefault()
        {
            return this.GetDefinition("default", "default");
        }

        public string GetFileCacheStorageConnectionString()
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

        public string GetPageLink(PAGE_LINK_TYPES type)
        {
            throw new NotImplementedException();
        }

        public SignApiServiceOptions GetSignApiServiceOptions()
        {
            var options = new SignApiServiceOptions(this.Options.ServiceSignUrl, this.Options.ServiceSignUser, this.Options.ServiceSignPassword);
            return options;
        }

        public SiteSettingsModel GetSiteSettings()
        {
            throw new NotImplementedException();
        }

        public ProcessStepModel[] GetSteps(ProcessStepModel currentStep)
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

        protected IEnumerable<DefinitionCombinationModel> GetAllDefinitionCombinations()
        {
            var list = new List<DefinitionCombinationModel>();
            // 8003 Indi
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "8003");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // 8003 SBN
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "8003");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "C");
                list.Add(def);
            }
            // Akvizice indi
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "02");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // Akvizice kampan
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "02");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "B");
                list.Add(def);
            }
            // Akvizice kampan
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "02");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "C");
                list.Add(def);
            }
            // BNR ikarta
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "I");
                list.Add(def);
            }
            // BNR investor
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "G");
                list.Add(def);
            }
            // BNR NSZ
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "H");
                list.Add(def);
            }
            // BNR pojisteni
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "D");
                list.Add(def);
            }
            // BNR senzor
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "E");
                list.Add(def);
            }
            // BNR SPS
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "04");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "F");
                list.Add(def);
            }
            // Refix indi
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "00");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // Refix kampan
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "00");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "B");
                list.Add(def);
            }
            // Retence indi
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "01");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "A");
                list.Add(def);
            }
            // Retence kampan
            {
                var def = new DefinitionCombinationModel();
                def.Process = this.GetAllProcesses().First(x => x.Code == "01");
                def.ProcessType = this.GetAllProcessTypes().First(x => x.Code == "B");
                list.Add(def);
            }
            return list;
        }
    }
}
