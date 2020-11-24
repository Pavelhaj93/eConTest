using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonDocumentsOthersCommoditiesModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        [JsonProperty("params")]
        public JsonParamModel[] Params { get; set; }

        [JsonProperty("arguments")]
        public JsonArgumentModel[] Arguments { get; set; }

        [JsonProperty("subTitle2")]
        public string SubTitle2 { get; set; }

        [JsonProperty("mandatory")]
        public int Mandatory { get; set; }

        [JsonProperty("files")]
        public JsonAcceptFileModel[] Files { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
