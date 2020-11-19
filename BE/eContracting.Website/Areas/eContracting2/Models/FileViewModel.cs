using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
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
        public int Size { get; set; }

        public FileViewModel()
        {
        }

        public FileViewModel(OfferAttachmentModel attachment)
        {
            this.Label = attachment.FileName;
            this.UniqueId = attachment.UniqueKey;
            this.MimeType = attachment.MimeType;
            this.Size = attachment.Size;
        }
    }
}
