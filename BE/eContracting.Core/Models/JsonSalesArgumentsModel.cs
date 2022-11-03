using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonSalesArgumentsModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public IEnumerable<JsonArgumentModel> Arguments { get; set; } = Enumerable.Empty<JsonArgumentModel>();

        [JsonProperty("commodityProductType", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string CommodityProductType { get; set; }
    }
}
