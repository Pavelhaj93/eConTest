using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Contains data from AD1 - <c>COMMODITY_OFFER_SUMMARY</c>.
    /// </summary>
    public class JsonOfferPerexModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public JsonParamModel[] Parameters { get; set; }
    }
}
