using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonGiftsModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("groups")]
        public IEnumerable<JsonGiftsGroupModel> Groups { get; set; }
    }
}
