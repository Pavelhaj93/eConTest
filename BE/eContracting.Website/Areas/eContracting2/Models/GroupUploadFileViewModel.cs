using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class GroupUploadFileViewModel
    {
        /// <summary>
        /// Gets or sets file unique key.
        /// </summary>
        [JsonProperty("key")]
        public readonly string Key;

        /// <summary>
        /// Gets or sets name of the file.
        /// </summary>
        [JsonProperty("label")]
        public readonly string Label;

        /// <summary>
        /// Gets or sets file MIME type.
        /// </summary>
        [JsonProperty("mime")]
        public readonly string MimeType;

        /// <summary>
        /// Gets or sets original file size.
        /// </summary>
        [JsonProperty("size")]
        public readonly long Size;

        public GroupUploadFileViewModel(FileInOptimizedGroupModel model)
        {
            this.Label = model.FileName;
            this.Key = model.Key;
            this.MimeType = model.MimeType;
            this.Size = model.OriginalSize;
        }
    }
}
