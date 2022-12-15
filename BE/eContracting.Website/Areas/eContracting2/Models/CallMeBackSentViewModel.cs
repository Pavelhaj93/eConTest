using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class CallMeBackSentViewModel
    {
        [JsonProperty("succeed")]
        public bool Succeeded { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("labels")]
        public IDictionary<string, string> Labels { get; } = new Dictionary<string, string>();
    }
}
