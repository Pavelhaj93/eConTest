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

        [JsonProperty("mandatory")]
        public int Mandatory { get; set; }

        [JsonProperty("files")]
        public IEnumerable<JsonAcceptFileModel> Files { get; set; }
    }
}
