using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsOthersModel
    {
        [JsonProperty("services")]
        public readonly JsonDocumentsAdditionalServicesModel AdditionalServices;

        [JsonProperty("products")]
        public readonly JsonDocumentsOtherProductsModel OtherProducts;

        public JsonDocumentsOthersModel(JsonDocumentsOtherProductsModel otherProducts, JsonDocumentsAdditionalServicesModel additionalServices)
        {
            this.OtherProducts = otherProducts;
            this.AdditionalServices = additionalServices;
        }
    }
}
