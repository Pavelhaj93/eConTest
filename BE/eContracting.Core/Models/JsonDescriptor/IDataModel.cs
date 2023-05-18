using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public interface IDataModel
    {
        [JsonProperty("type")]
        string Type { get; }

        [JsonProperty("position")]
        int Position { get; set; }
        
        [JsonProperty("header")]
        IDataHeaderModel Header { get; set; }

        [JsonProperty("body")]
        IDataBodyModel Body { get; set; }
    }
}
