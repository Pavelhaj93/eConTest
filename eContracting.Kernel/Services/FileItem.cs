using System;
using Newtonsoft.Json;

namespace eContracting.Kernel.Services
{
    public class FileItem
    {
        [JsonProperty("title")]
        public String Title { get; set; }

        [JsonProperty("url")]
        public String Url { get; set; }
    }
}
