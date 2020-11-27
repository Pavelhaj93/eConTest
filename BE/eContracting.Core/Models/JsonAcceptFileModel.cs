using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonAcceptFileModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("mime")]
        public string MimeType { get; set; }

        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; } = true;
    }
}
