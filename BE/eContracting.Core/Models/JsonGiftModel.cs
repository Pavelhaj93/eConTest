using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonGiftModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
