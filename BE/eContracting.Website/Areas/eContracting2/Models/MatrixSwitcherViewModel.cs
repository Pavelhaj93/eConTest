using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using eContracting.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.DependencyInjection;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class MatrixSwitcherViewModel
    {
        public string SubmitUrl { get; set; }
        public NameValueCollection Query { get; set; }
        public DefinitionViewModel CurrentDefinition { get; set; }
        public DefinitionViewModel[] Definitions { get; set; }
        public bool IsPreview { get; set; }

        public MatrixSwitcherViewModel()
        {
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
        public class DefinitionViewModel
        {
            [JsonProperty("id")]
            public readonly Guid ID;

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("process")]
            public readonly DefinitionProcessViewModel Process;

            [JsonProperty("processType")]
            public readonly DefinitionProcessViewModel ProcessType;

            public DefinitionViewModel(IDefinitionCombinationModel definition)
            {
                this.ID = definition.ID;
                this.Name = definition.Name;
                this.Process = new DefinitionProcessViewModel(definition.Process);
                this.ProcessType = new DefinitionProcessViewModel(definition.ProcessType);
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptOut)]
        public class DefinitionProcessViewModel
        {
            [JsonProperty("name")]
            public readonly string Name;

            [JsonProperty("code")]
            public readonly string Code;

            public DefinitionProcessViewModel(IProcessModel process)
            {
                this.Name = process.Name;
                this.Code = process.Code;
            }

            public DefinitionProcessViewModel(IProcessTypeModel processType)
            {
                this.Name = processType.Name;
                this.Code = processType.Code;
            }
        }
    }
}
