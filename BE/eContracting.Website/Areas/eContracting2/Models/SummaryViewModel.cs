using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class SummaryViewModel : BaseAppConfigViewModel
    {
        [JsonIgnore]
        public StepsViewModel Steps { get; set; }

        [JsonIgnore]
        public string MainText { get; set; }

        [JsonProperty("getSummaryUrl")]
        public readonly string SummaryApi = "/api/econ/summary";

        [JsonProperty("keepAliveUrl")]
        public readonly string KeepAliveUrl = "/api/econ/keepalive";

        [JsonProperty("getCallMeBackUrl")]
        public string CmbGetUrl { get; set; } = "/api/econ/getscmb";

        [JsonProperty("postCallMeBackUrl")]
        public string CmbPostUrl { get; set; } = "/api/econ/sendscmb";

        [JsonProperty("offerUrl")]
        public string OfferPage { get; set; }

        [JsonProperty("authUrl")]
        public string LoginPage { get; set; }

        [JsonIgnore]
        public IDefinitionCombinationModel Definition { get; set; }

        public SummaryViewModel(ISiteSettingsModel siteSettings, string guid) : base("Summary", siteSettings, guid)
        {
        }
    }
}
