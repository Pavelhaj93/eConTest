using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class TitleAndValuesModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("values")]
        public IEnumerable<string> Values { get; set; }
    }
}
