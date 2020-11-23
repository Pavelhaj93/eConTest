using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
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
            var options = new CacheApiServiceOptions(this.Options.Value.ServiceUrl, this.Options.Value.ServiceUser, this.Options.Value.ServicePassword);
            return options;
        }

        public DefinitionCombinationModel GetDefinition(OfferModel offer)
        {
            return this.GetDefinition(offer.Process, offer.ProcessType);
        }

        public DefinitionCombinationModel GetDefinition(string process, string processType)
        {
            return this.GetAllDefinitionCombinations().FirstOrDefault(x => x.Process.Code == process && x.ProcessType.Code == processType);
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

        public SiteSettingsModel GetSiteSettings()
        {
            throw new NotImplementedException();
        }

        public ProcessStepModel[] GetSteps(ProcessStepModel currentStep)
        {
            throw new NotImplementedException();
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
