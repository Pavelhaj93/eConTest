using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonArgumentModel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        public JsonArgumentModel()
        {
        }

        public JsonArgumentModel(string value)
        {
            this.Value = value;
        }
    }
}
