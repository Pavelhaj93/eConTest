using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonUploadTemplateModel
    {
        [JsonProperty("id")]
        public string GroupId { get; set; }

        [JsonIgnore]
        public string IdAttach { get; set; }

        [JsonIgnore]
        public string Product { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; }

        [JsonProperty("idx")]
        public int Position { get; set; }

        public JsonUploadTemplateModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonUploadTemplateModel"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <exception cref="ArgumentNullException">template</exception>
        public JsonUploadTemplateModel(OfferAttachmentModel template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            this.GroupId = template.UniqueKey;
            this.IdAttach = template.IdAttach;
            this.Product = template.Product;
            this.Title = template.FileName;
            this.Mandatory = template.IsRequired;
        }
    }
}
