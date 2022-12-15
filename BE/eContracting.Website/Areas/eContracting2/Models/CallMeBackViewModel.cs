using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class CallMeBackViewModel
    {
        [JsonProperty("succeed")]
        public bool Succeeded { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("image")]
        public ImageViewModel Image { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("maxFileSize")]
        public int MaxFileSize { get; set; }

        [JsonProperty("maxFiles")]
        public int MaxFiles { get; set; }

        [JsonProperty("allowedFiles")]
        public IEnumerable<string> AllowedFileTypes { get; set; }

        [JsonProperty("times")]
        public IEnumerable<SelectionViewModel> AllowedTimes { get; set; }

        [JsonProperty("labels")]
        public IDictionary<string, string> Labels { get; } = new Dictionary<string, string>();
    }
}
