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
        [JsonProperty("perex")]
        public JsonOfferPerexModel Perex { get; set; }

        [JsonProperty("gifts")]
        public JsonOfferGiftsModel Gifts { get; set; }

        [JsonProperty("benefits")]
        public JsonOfferBenefitsModel Benefits { get; set; }

        [JsonProperty("documents")]
        public JsonOfferDocumentsModel Documents { get; set; }

        [JsonProperty("acceptance")]
        public JsonAcceptanceDialogViewModel AcceptanceDialog { get; set; }
    }
}
