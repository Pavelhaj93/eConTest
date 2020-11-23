using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsAcceptanceModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("accept")]
        public JsonDocumentsAcceptModel Accept { get; set; }

        [JsonProperty("sign")]
        public JsonDocumentsAcceptModel Sign { get; set; }
    }
}
