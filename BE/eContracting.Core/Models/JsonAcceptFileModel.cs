using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonAcceptFileModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonIgnore]
        public string IdAttach { get; set; }

        [JsonIgnore]
        public string Product { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("mime")]
        public string MimeType { get; set; }

        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAcceptFileModel"/> class.
        /// </summary>
        public JsonAcceptFileModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAcceptFileModel"/> class.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <exception cref="ArgumentNullException">attachment</exception>
        public JsonAcceptFileModel(OfferAttachmentModel attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            this.Key = attachment.UniqueKey;
            this.Group = attachment.GroupGuid;
            this.IdAttach = attachment.IdAttach;
            this.Product = attachment.Product;
            this.Label = attachment.FileName;
            this.MimeType = attachment.MimeType;
            this.Mandatory = attachment.IsRequired;
        }
    }
}
