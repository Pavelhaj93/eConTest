using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsAdditionalServicesModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("mandatoryGroups")]
        public HashSet<string> MandatoryGroups { get; } = new HashSet<string>();

        [JsonProperty("files")]
        public IEnumerable<JsonAcceptFileModel> Files { get; set; }
    }
}
