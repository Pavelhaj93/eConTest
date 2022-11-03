using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsOtherProductsModel
    {
        [JsonProperty("sectionInfo", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public JsonSectionAcceptanceBoxModel AcceptanceInfoBox { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("params")]
        public IEnumerable<JsonParamModel> Params { get; set; }

        [JsonProperty("arguments")]
        public IEnumerable<JsonArgumentModel> SalesArguments { get; set; }

        [JsonProperty("subTitle2")]
        public string SubTitle2 { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("mandatoryGroups")]
        public HashSet<string> MandatoryGroups { get; } = new HashSet<string>();

        [JsonProperty("files")]
        public IEnumerable<JsonAcceptFileModel> Files { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
