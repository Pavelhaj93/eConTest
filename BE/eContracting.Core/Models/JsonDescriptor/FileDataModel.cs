using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class FileDataModel
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

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("mime")]
        public string MimeType { get; set; }

        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; } = true;

        [JsonProperty("idx")]
        public int Position { get; set; }

        [JsonIgnore]
        public IProductInfoModel MatchedProductInfo { get; set; }


        /// <summary>
        /// Gets all XML template file attributes as collection.
        /// </summary>
        /// <see cref="OfferAttachmentXmlModel.XmlTemplateAttributes"/>
        [JsonIgnore]
        public readonly IDictionary<string, string> XmlAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAcceptFileModel"/> class.
        /// </summary>
        public FileDataModel()
        {
            this.XmlAttributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataModel"/> class.
        /// </summary>
        /// <param name="attachment">The attachment.</param>
        /// <exception cref="ArgumentNullException">attachment</exception>
        public FileDataModel(OfferAttachmentModel attachment)
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
            this.Position = attachment.Position;

            this.XmlAttributes = attachment.DocumentTemplate.XmlTemplateAttributes;
        }
    }
}
