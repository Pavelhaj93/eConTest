using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ImageViewModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
