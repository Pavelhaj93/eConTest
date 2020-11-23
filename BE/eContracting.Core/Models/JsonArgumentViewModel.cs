using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonArgumentViewModel
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
