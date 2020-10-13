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
        [JsonProperty("doxReadyUrl")]
        public string doxReadyUrl { get; set; }

        [JsonProperty("isAgreed")]
        public bool IsAgreed { get; set; }

        [JsonProperty("isRetention")]
        [Obsolete]
        public bool IsRetention { get; set; }

        [JsonProperty("isAcquisition")]
        [Obsolete]
        public bool IsAcquisition { get; set; }

        [JsonProperty("formAction")]
        public string FormAction { get; set; }

        [JsonProperty("itemValue")]
        public string HiddenValue { get; set; }

        [JsonProperty("choices")]
        public IEnumerable<LoginChoiceViewModel> Choices { get; set; }

        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; set; }

        public LoginViewModel() : base("Authentication")
        {
        }
    }
}
