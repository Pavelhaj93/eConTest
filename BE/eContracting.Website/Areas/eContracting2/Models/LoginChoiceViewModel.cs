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

        [JsonProperty("id")]
        public string Id { get; set; }

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

        public LoginChoiceViewModel(LoginTypeModel model)
        {
            this.HelpText = model.HelpText;
            this.Id = model.ComputeId;
            this.Key = model.Key;
            this.Label = model.Label;
            this.Placeholder = model.Placeholder;
            this.ValidationRegex = model.ValidationRegex;
        }
    }
}
