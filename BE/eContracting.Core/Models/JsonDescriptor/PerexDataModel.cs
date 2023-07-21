using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class PerexDataModel
    {
        [JsonProperty("header")]
        public IDataHeaderModel Header { get; set; }
        
        [JsonProperty("body")]
        public PerexBodyModel Body { get; set; }
    }
}
