using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class DocumentDataBodyModel : IDataBodyModel
    {
        [JsonProperty("head")]
        public DocumentDataBodyHeadModel Head { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("docs")]
        public DocsDataModel Docs { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
