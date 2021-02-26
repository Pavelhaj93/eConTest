using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;
using eContracting.Storage;
using Newtonsoft.Json;
using Sitecore.IO;

namespace eContracting.Models
{
    /// <summary>
    /// Represents file from 'ZCCH_CACHE_API'.
    /// </summary>
    [Serializable]
    public class OfferAttachmentModel
    {
        /// <summary>
        /// The unique key.
        /// </summary>
        [JsonProperty("key")]
        public readonly string UniqueKey;

        /// <summary>
        /// The identifier of the attachment. 
        /// </summary>
        [JsonProperty("idattach")]
        public readonly string IdAttach;

        [JsonProperty("product")]
        public readonly string Product;

        /// <summary>
        /// Gets or set index.
        /// </summary>
        [JsonProperty("index")]
        public readonly string Index;

        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        [JsonProperty("mimeType")]
        public readonly string MimeType;

        /// <summary>
        /// The original file name generated from SAP (technical name).
        /// </summary>
        [JsonIgnore]
        public readonly string OriginalFileName;

        /// <summary>
        /// The file name.
        /// </summary>
        [JsonProperty("name")]
        public readonly string FileName;

        /// <summary>
        /// The file extension
        /// </summary>
        [JsonIgnore]
        public readonly string FileExtension;

        /// <summary>
        /// The file name with extension.
        /// </summary>
        [JsonProperty("file")]
        public readonly string FileNameExtension;

        /// <summary>
        /// The description.
        /// </summary>
        [JsonProperty("description")]
        public readonly string Description;

        /// <summary>
        /// Gets value of attribute "GROUP" or null. Expected values: KOM1, SEN1, INV1.
        /// </summary>
        [JsonProperty("group")]
        public readonly string Group;

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.GROUP_OBLIG"/> with value 'X'.
        /// </summary>
        [JsonProperty("isGroupObligatory")]
        public readonly bool IsGroupObligatory;

        /// <summary>
        /// Determinates if this attachment is obligatory or not.
        /// </summary>
        [JsonProperty("isObligatory")]
        public readonly bool IsObligatory;

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.PRINTED"/> with value 'X'.
        /// </summary>
        [JsonProperty("isPrinted")]
        public readonly bool IsPrinted;

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.SIGN_REQ"/> with value 'X'.
        /// </summary>
        [JsonProperty("isSignReq")]
        public readonly bool IsSignReq;

        /// <summary>
        /// Gets or sets a value indicating whether this version of document is signed.
        /// </summary>
        [JsonProperty("isSigned")]
        public readonly bool SignedVersion;

        /// <summary>
        /// Gets or sets file content. Always returns array.
        /// </summary>
        [JsonIgnore]
        public readonly byte[] FileContent;

        /// <summary>
        /// The file attributes. Always returns array.
        /// </summary>
        [JsonIgnore]
        public readonly OfferAttributeModel[] Attributes;

        /// <summary>
        /// Group in which the template is.
        /// </summary>
        public readonly string GroupGuid;

        /// <summary>
        /// The consent type identifier (expected S or P).
        /// </summary>
        public readonly string ConsentType;

        /// <summary>
        /// The template alc identifier
        /// </summary>
        public readonly string TemplAlcId;

        public readonly OfferAttachmentXmlModel DocumentTemplate;

        /// <summary>
        /// Gets <c>true</c> is <see cref="IsObligatory"/> == true OR <see cref="IsGroupObligatory"/> == true.
        /// </summary>
        [JsonProperty("isRequired")]
        public bool IsRequired
        {
            get
            {
                return this.IsObligatory || this.IsGroupObligatory;
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        [JsonIgnore]
        public int Size
        {
            get
            {
                return this.FileContent?.Length ?? 0;
            }
        }

        /// <summary>
        /// Gets size description.
        /// </summary>
        [JsonProperty("size")]
        public string SizeLabel
        {
            get
            {
                return Utils.GetReadableFileSize(this.Size);
            }
        }

        [JsonProperty("position")]
        public int Position
        {
            get
            {
                if (int.TryParse(this.DocumentTemplate.SequenceNumber, out int pos))
                {
                    return pos;
                }

                return 0;
            }
        }

        #region Desicion makers where to place a file

        /// <summary>
        /// Gets value of attribute "TEMPLATE" or null. Expected values: EES, A10, V01, V02, V03, EPO, AD1, ZPE, ED2, EP2X, DS1, DSE, V04, DP6, DP3.
        /// </summary>
        [JsonProperty("template")]
        public string Template
        {
            get
            {
                return this.Attributes?.FirstOrDefault(x => x.Key == Constants.FileAttributes.TEMPLATE)?.Value;
            }
        }

        #endregion

        /// <summary>
        /// Prevents a default instance of the <see cref="OfferAttachmentModel"/> class from being created.
        /// </summary>
        private OfferAttachmentModel()
        {
            this.Attributes = new OfferAttributeModel[] { };
            this.FileContent = new byte[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentModel"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        public OfferAttachmentModel(OfferAttachmentXmlModel template) : this(template, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentModel"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="originalFileName">Name of the original file.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="content">The content.</param>
        public OfferAttachmentModel(OfferAttachmentXmlModel template, string mimeType, string originalFileName, OfferAttributeModel[] attributes, byte[] content)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            this.DocumentTemplate = template;

            if (string.IsNullOrEmpty(originalFileName))
            {
                originalFileName = template.Description;
            }

            if (string.IsNullOrEmpty(originalFileName))
            {
                throw new ArgumentException("Invalid file name (empty name)");
            }

            this.OriginalFileName = originalFileName;

            this.Group = template.Group;
            this.GroupGuid = template.ItemGuid;
            this.FileName = template.Description;
            this.ConsentType = template.ConsentType;
            this.TemplAlcId = template.TemplAlcId;
            this.IsObligatory = template.IsObligatory();
            this.IsGroupObligatory = template.IsGroupObligatory();
            this.IsPrinted = template.IsPrinted();
            this.IsSignReq = template.IsSignRequired();
            this.IdAttach = template.IdAttach;
            this.Product = template.Product;
            this.UniqueKey = template.UniqueKey;

            this.MimeType = mimeType;
            this.FileExtension = Path.GetExtension(this.OriginalFileName).TrimStart('.');
            this.FileNameExtension = template.Description + "." + this.FileExtension;
            this.Attributes = attributes ?? new OfferAttributeModel[] { };
            this.FileContent = content ?? new byte[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentModel"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="file">The file.</param>
        /// <param name="fileName">Readable file name.</param>
        /// <param name="attributes">The attributes.</param>
        /// <exception cref="ArgumentNullException">template</exception>
        /// <exception cref="ArgumentException">Invalid file name (empty name)</exception>
        public OfferAttachmentModel(OfferAttachmentXmlModel template, OfferFileXmlModel file, string fileName, OfferAttributeModel[] attributes)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            this.DocumentTemplate = template;
            var originalFileName = file.File.FILENAME;

            if (string.IsNullOrEmpty(originalFileName))
            {
                originalFileName = template.Description;
            }

            if (string.IsNullOrEmpty(originalFileName))
            {
                throw new ArgumentException("Invalid file name (empty name)");
            }

            this.OriginalFileName = originalFileName;
            this.FileName = fileName;

            this.Group = template.Group;
            this.GroupGuid = template.ItemGuid;
            this.ConsentType = template.ConsentType;
            this.TemplAlcId = template.TemplAlcId;
            this.IsObligatory = template.IsObligatory();
            this.IsGroupObligatory = template.IsGroupObligatory();
            this.IsPrinted = template.IsPrinted();
            this.IsSignReq = template.IsSignRequired();
            this.IdAttach = template.IdAttach;
            this.Product = template.Product;
            this.UniqueKey = template.UniqueKey;

            this.MimeType = file.File.MIMETYPE;
            this.FileExtension = Path.GetExtension(this.OriginalFileName).TrimStart('.');
            this.FileNameExtension = template.Description + "." + this.FileExtension;
            this.Attributes = attributes ?? new OfferAttributeModel[] { };
            this.FileContent = file.File.FILECONTENT ?? new byte[] { };

        }

        /// <summary>
        /// Determines whether an attribute with <paramref name="key"/> has value 'X'.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if is attribute has value 'X', otherwise <c>false</c>.
        /// </returns>
        public bool IsAttributeChecked(string key)
        {
            var attr = this.Attributes.FirstOrDefault(x => x.Key == key);

            if (attr == null)
            {
                return false;
            }

            return attr.Value.Equals("X", StringComparison.InvariantCultureIgnoreCase);
        }

        public OfferAttachmentModel Clone(byte[] newFileContent)
        {
            var template = new OfferAttachmentXmlModel();
            template.AddInfo = this.DocumentTemplate.AddInfo;
            template.ConsentType = this.DocumentTemplate.ConsentType;
            template.Description = this.DocumentTemplate.Description;
            template.Group = this.DocumentTemplate.Group;
            template.GroupObligatory = this.DocumentTemplate.GroupObligatory;
            template.IdAttach = this.DocumentTemplate.IdAttach;
            template.Product = this.DocumentTemplate.Product;
            template.ItemGuid = this.DocumentTemplate.ItemGuid;
            template.Obligatory = this.DocumentTemplate.Obligatory;
            template.Printed = this.DocumentTemplate.Printed;
            template.SequenceNumber = this.DocumentTemplate.SequenceNumber;
            template.SignReq = this.DocumentTemplate.SignReq;
            template.TemplAlcId = this.DocumentTemplate.TemplAlcId;
            template.TimeStampRequired = this.DocumentTemplate.TimeStampRequired;

            var model = new OfferAttachmentModel(template, this.MimeType, this.OriginalFileName, this.Attributes, newFileContent);
            return model;
        }

        protected string GetFileNameExtension(OfferAttachmentXmlModel template, string originalFileName)
        {
            var ext = Path.GetExtension(originalFileName);
            return template.Description + ext;
        }
    }
}
