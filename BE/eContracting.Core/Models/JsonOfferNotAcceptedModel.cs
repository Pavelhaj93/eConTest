using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferNotAcceptedModel
    {
        [JsonProperty("Message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("perex", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonOfferPerexModel Perex { get; set; }

        [JsonProperty("benefits", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSalesArgumentsModel Benefits { get; set; }

        [JsonProperty("gifts", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonAllBenefitsModel Gifts { get; set; }

        [JsonProperty("documents", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonOfferDocumentsModel Documents { get; set; }

        [JsonProperty("acceptance")]
        public JsonAcceptanceDialogViewModel AcceptanceDialog { get; set; }
    }
}
