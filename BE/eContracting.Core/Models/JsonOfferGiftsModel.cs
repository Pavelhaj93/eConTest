using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferGiftsModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public JsonArgumentViewModel[] Params { get; set; }
    }
}
