using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferDocumentsModel
    {
        [JsonProperty("acceptance", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public JsonDocumentsAcceptanceModel Acceptance { get; set; }

        [JsonProperty("uploads", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public JsonDocumentsUploadsModel Uploads { get; set; }

        [JsonProperty("other", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public JsonDocumentsOthersModel Other { get; set; }
    }
}
