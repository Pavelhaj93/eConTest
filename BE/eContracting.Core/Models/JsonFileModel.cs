using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonFileModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("key")]
        public string UniqueId { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        public JsonFileModel()
        {
        }

        public JsonFileModel(OfferAttachmentModel attachment)
        {
            this.Label = attachment.FileName;
            this.UniqueId = attachment.UniqueKey;
            this.MimeType = attachment.MimeType;
            this.Size = attachment.Size;
        }
    }
}
