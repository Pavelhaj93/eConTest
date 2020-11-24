using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonGiftsGroupModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public IEnumerable<JsonGiftModel> Params { get; set; }
    }
}
