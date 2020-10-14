using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class LoginChoiceViewModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("helpText")]
        public string HelpText { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("regex")]
        public string ValidationRegex { get; set; }

        public LoginChoiceViewModel()
        {
        }

        public LoginChoiceViewModel(LoginTypeModel model, string key)
        {
            this.HelpText = model.HelpText;
            this.Key = key;
            this.Label = model.Label;
            this.Placeholder = model.Placeholder;
            this.ValidationRegex = model.ValidationRegex;
        }
    }
}
