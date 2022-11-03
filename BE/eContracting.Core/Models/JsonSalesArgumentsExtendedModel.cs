using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonSalesArgumentsExtendedModel : JsonSalesArgumentsModel
    {
        [JsonProperty("summary", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<JsonParamModel> Summary { get; set; }
    }
}
