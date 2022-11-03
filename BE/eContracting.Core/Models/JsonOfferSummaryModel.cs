using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferSummaryModel
    {
        [JsonProperty("personal", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonOfferPersonalDataModel User { get; set; }

        [JsonProperty("distributor_change", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonOfferDistributorChangeModel DistributorChange { get; set; }

        [JsonProperty("product", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonOfferProductModel Product { get; set; }

        [JsonProperty("benefits", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<JsonSalesArgumentsModel> SalesArguments { get; set; }

        [JsonProperty("gifts", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonAllBenefitsModel Gifts { get; set; }
    }
}
