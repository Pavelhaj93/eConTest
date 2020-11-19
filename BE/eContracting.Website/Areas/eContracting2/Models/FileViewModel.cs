using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class FileViewModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("key")]
        public string UniqueId { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("size")]
        public string SizeLabel { get; set; }
    }
}
