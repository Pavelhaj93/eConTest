using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class GroupUploadFileViewModel
    {
        /// <summary>
        /// Gets or sets name of the file.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets file unique key.
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets file MIME type.
        /// </summary>
        [JsonProperty("mime")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets original file size.
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }
    }
}
