using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class PerexBodyModel : IDataBodyModel
    {
        [JsonProperty("params")]
        public IEnumerable<TitleAndValueModel> Params { get; set; }
    }
}
