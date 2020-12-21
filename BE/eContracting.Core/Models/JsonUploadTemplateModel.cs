using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonUploadTemplateModel
    {
        [JsonProperty("id")]
        public string GroupId { get; set; }

        [JsonIgnore]
        public string IdAttach { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; }
    }
}
