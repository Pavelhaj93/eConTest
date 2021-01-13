using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsUploadsModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("mandatoryGroups")]
        public HashSet<string> MandatoryGroups { get; } = new HashSet<string>();

        [JsonProperty("types")]
        public IEnumerable<JsonUploadTemplateModel> Types { get; set; }
    }
}
