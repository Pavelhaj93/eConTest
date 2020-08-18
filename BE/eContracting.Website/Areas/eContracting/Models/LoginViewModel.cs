using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting.Models
{
    public class LoginViewModel : BaseViewModel
    {
        [JsonProperty("doxReadyUrl")]
        public string doxReadyUrl { get; set; }

        [JsonProperty("isAgreed")]
        public bool IsAgreed { get; set; }

        [JsonProperty("isRetention")]
        public bool IsRetention { get; set; }

        [JsonProperty("isAcquisition")]
        public bool IsAcquisition { get; set; }

        [JsonProperty("formAction")]
        public string FormAction { get; set; }

        [JsonProperty("choices")]
        public List<LoginChoiceViewModel> Choices { get; set; }

        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; set; }

        public LoginViewModel() : base("Authentication")
        {
        }
    }

    public class LoginChoiceViewModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("helpText")]
        public string HelpText { get; set; }

        [JsonProperty("placeholder")]
        public string Placeholder { get; set; }

        [JsonProperty("regex")]
        public string ValidationRegex { get; set; }
    }
}
