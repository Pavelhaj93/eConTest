using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsAcceptModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        [JsonProperty("mandatoryGroups")]
        public HashSet<string> MandatoryGroups { get; } = new HashSet<string>();

        [JsonProperty("files")]
        public IEnumerable<JsonAcceptFileModel> Files { get; set; }

        [JsonProperty("note", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }
    }
}
