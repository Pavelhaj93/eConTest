using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferDocumentsModel
    {
        [JsonProperty("acceptance")]
        public JsonDocumentsAcceptanceModel Acceptance { get; set; }

        [JsonProperty("uploads")]
        public JsonDocumentsUploadsModel Uploads { get; set; }

        [JsonProperty("other")]
        public JsonDocumentsOthersModel Other { get; set; }
    }
}
