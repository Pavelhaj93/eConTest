using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class ProductDataPricesModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("price2")]
        public string Price2 { get; set; }

        [JsonProperty("Unit")]
        public string Unit { get; set; }
    }
}
