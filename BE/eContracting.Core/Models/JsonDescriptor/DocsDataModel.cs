using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class DocsDataModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public IEnumerable<TitleAndValueModel> Params { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("mandatoryGroups")]
        public HashSet<string> MandatoryGroups { get; } = new HashSet<string>();

        [JsonProperty("files")]
        public IEnumerable<FileDataModel> Files { get; set; }
    }
}

