using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsOthersModel
    {
        [JsonProperty("commodities")]
        public JsonDocumentsOthersCommoditiesModel Commodities { get; set; }

        [JsonProperty("services")]
        public JsonDocumentsOthersServicesModel Services { get; set; }
    }
}
