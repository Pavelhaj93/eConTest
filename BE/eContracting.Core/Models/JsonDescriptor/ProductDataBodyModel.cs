using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class ProductDataBodyModel : IDataBodyModel
    {
        [JsonProperty("prices")]
        public IEnumerable<ProductDataPricesModel> Prices { get; set; }

        [JsonProperty("infos")]
        public IEnumerable<ValueModel> Infos { get; set; }

        [JsonProperty("infoHelp", NullValueHandling = NullValueHandling.Ignore)]
        public string InfoHelp { get; set; }

        [JsonProperty("points")]
        public IEnumerable<ValueModel> Points { get; set; }        
    }
}
