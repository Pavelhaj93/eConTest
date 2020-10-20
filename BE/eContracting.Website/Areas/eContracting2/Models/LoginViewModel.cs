﻿using System;
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
        public LoginPageModel Datasource { get; set; }

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

        public string BussProcess { get; set; }
        public string BussProcessType { get; set; }
        public string Birthdate { get; set; }
        public string Partner { get; set; }
        public string Zip1 { get; set; }
        public string Zip2 { get; set; }

        [JsonProperty("choices")]
        public IEnumerable<LoginChoiceViewModel> Choices { get; set; }

        public IEnumerable<ProcessStepModel> Steps { get; set; }

        [JsonProperty("labels")]
        public Dictionary<string, string> Labels { get; set; }

        public LoginViewModel() : base("Authentication")
        {
        }
    }
}
