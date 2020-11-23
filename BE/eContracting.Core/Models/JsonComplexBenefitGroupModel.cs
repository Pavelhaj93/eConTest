using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonComplexBenefitGroupModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public JsonBenefitModel[] Params { get; set; }
    }
}
