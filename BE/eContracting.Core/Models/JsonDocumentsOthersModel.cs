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
        public readonly JsonDocumentsOthersCommoditiesModel Commodities;

        [JsonProperty("services")]
        public readonly JsonDocumentsOthersServicesModel;

        public JsonDocumentsOthersModel(JsonDocumentsOthersCommoditiesModel commodities, JsonDocumentsOthersServicesModel services)
        {
            this.Commodities = commodities;
            this.Services = services;
        }
    }
}
