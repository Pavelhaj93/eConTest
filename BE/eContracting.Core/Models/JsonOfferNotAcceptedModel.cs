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
        [JsonProperty("perex", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonOfferPerexModel Perex { get; set; }

        [JsonProperty("benefits", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonBenefitsModel Benefits { get; set; }

        [JsonProperty("gifts", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonGiftsModel Gifts { get; set; }

        [JsonProperty("documents")]
        public JsonOfferDocumentsModel Documents { get; set; }

        [JsonProperty("acceptance")]
        public JsonAcceptanceDialogViewModel AcceptanceDialog { get; set; }
    }
}
