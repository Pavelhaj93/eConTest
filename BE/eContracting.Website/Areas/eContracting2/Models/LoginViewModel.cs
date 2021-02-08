using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class LoginViewModel : BaseViewModel
    {
        [JsonIgnore]
        public readonly PageLoginModel Datasource;

        [JsonIgnore]
        public readonly StepsViewModel Steps;

        [JsonIgnore]
        public string MainText { get; set; }

        [JsonProperty("choices")]
        public readonly IEnumerable<LoginChoiceViewModel> Choices;

        public readonly DefinitionCombinationModel Definition;

        /// <summary>
        /// Gets placeholder sufix with process and process type ("_{<see cref="DefinitionCombinationModel.Process"/>}_{<see cref="DefinitionCombinationModel.ProcessType"/>}")
        /// or empty string when <see cref="DefinitionCombinationModel.IsDefault"/> equals true.
        /// </summary>
        public string PlaceholderSufix
        {
            get
            {
                if (this.Definition.IsDefault)
                {
                    return "";
                }
                else
                {
                    return "_" + this.Definition.Process.Code + "_" + this.Definition.ProcessType.Code;
                }
            }
        }

        [JsonProperty("formAction")]
        public string FormAction { get; set; }

        [JsonProperty("itemValue")]
        public string HiddenValue { get; set; }

        public string BussProcess { get; set; }
        public string BussProcessType { get; set; }
        public string Birthdate { get; set; }
        public string Partner { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }

        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; set; }

        public LoginViewModel(DefinitionCombinationModel definition, PageLoginModel datasource, StepsViewModel steps, LoginChoiceViewModel[] choices) : base("Authentication")
        {
            this.Definition = definition;
            this.Datasource = datasource ?? throw new ArgumentNullException(nameof(datasource));
            this.Steps = steps ?? throw new ArgumentNullException(nameof(steps));
            this.Choices = choices ?? throw new ArgumentNullException(nameof(choices));
        }
    }
}
