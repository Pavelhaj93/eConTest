using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ThankYouViewModel
    {
        [JsonIgnore]
        public readonly ThankYouPageModel Datasource;

        [JsonIgnore]
        public readonly StepsViewModel Steps;

        [JsonIgnore]
        public string MainText { get; set; }

        public Dictionary<string, string> ScriptParameters { get; } = new Dictionary<string, string>();

        public ThankYouViewModel(ThankYouPageModel datasource, StepsViewModel steps)
        {
            this.Datasource = datasource;
            this.Steps = steps;
        }
    }
}
