using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonParamModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public JsonParamModel()
        {
        }

        public JsonParamModel(string title, string value)
        {
            this.Title = title;
            this.Value = value;
        }
    }
}
