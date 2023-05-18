using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class ValueModel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        public ValueModel()
        { }

        public ValueModel(string value)
        {
            this.Value = value;
        }
    }
}
