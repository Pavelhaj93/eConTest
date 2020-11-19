using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;
using Newtonsoft.Json;
using Sitecore.IO;

namespace eContracting.Models
{
    /// <summary>
    /// Represents file from 'ZCCH_CACHE_API'
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
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
        /// The original file name generated from SAP.
        /// </summary>
        [JsonIgnore]
        public readonly string OriginalFileName;

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        [JsonProperty("name")]
        public readonly string FileName;

        /// <summary>
        /// The file name with extension.
        /// </summary>
        [JsonProperty("file")]
        public readonly string FileNameExtension;

        /// <summary>
        /// Gets value of attribute "GROUP" or null. Expected values: KOM1, SEN1, INV1.
        /// </summary>
        [JsonProperty("group")]
        public readonly string Group;

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.GROUP_OBLIG"/> with value 'X'.
        /// </summary>
        [JsonProperty("isGroupObl")]
        public readonly bool IsGroupOblig;

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
        /// Gets or sets file content.
        /// </summary>
        [JsonIgnore]
        public readonly byte[] FileContent;

        /// <summary>
        /// The file attributes.
        /// </summary>
        [JsonIgnore]
        public readonly OfferAttributeModel[] Attributes;

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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentModel"/> class.
        /// </summary>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="idAttach">Id of the attachment.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="index">The index.</param>
        /// <param name="originalFileName">Original file name.</param>
        /// <param name="fileName">The labeled file name.</param>
        /// <param name="signedVersion">if set to <c>true</c> [signed version].</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="content">The content.</param>
        public OfferAttachmentModel(string uniqueKey, string idAttach, string mimeType, string index, string originalFileName, string fileName, bool signedVersion, OfferAttributeModel[] attributes, byte[] content)
        {
            this.UniqueKey = uniqueKey;
            this.IdAttach = idAttach;
            this.MimeType = mimeType;
            this.Index = index;
            this.OriginalFileName = originalFileName;
            this.FileName = fileName;
            this.SignedVersion = signedVersion;
            this.Attributes = attributes;
            this.FileContent = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentModel"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <exception cref="ArgumentNullException">template</exception>
        public OfferAttachmentModel(DocumentTemplateModel template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            this.Group = template.Group;
            this.FileName = template.Description;
            this.IsGroupOblig = template.GroupObligatory.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase);
            this.IsPrinted = template.Printed.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase);
            this.IsSignReq = template.SignReq.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentModel"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="idAttach">The identifier attach.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="originalFileName">Name of the original file.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="content">The content.</param>
        public OfferAttachmentModel(DocumentTemplateModel template, string uniqueKey, string idAttach, string mimeType, string originalFileName, OfferAttributeModel[] attributes, byte[] content) : this(template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            this.UniqueKey = uniqueKey;
            this.IdAttach = idAttach;
            this.MimeType = mimeType;
            this.OriginalFileName = originalFileName;
            this.FileNameExtension = this.GetFileNameExtension(template, originalFileName);
            this.Attributes = attributes;
            this.FileContent = content;
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

        protected string GetFileNameExtension(DocumentTemplateModel template, string originalFileName)
        {
            var ext = Path.GetExtension(originalFileName);
            return template.Description + ext;
        }
    }
}
