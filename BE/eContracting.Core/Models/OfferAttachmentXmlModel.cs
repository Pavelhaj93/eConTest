using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Represents file from 'ZCCH_CACHE_API'
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OfferAttachmentXmlModel
    {
        /// <summary>
        /// The unique key.
        /// </summary>
        [JsonProperty("key")]
        public readonly string UniqueKey;

        /// <summary>
        /// Gets or set index.
        /// </summary>
        [JsonProperty("index")]
        public readonly string Index;

        /// <summary>
        /// Gets or sets a file type.
        /// </summary>
        [JsonProperty("type")]
        public string FileType
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == Constants.FileAttributes.TYPE)?.Value;
            }
        }

        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        [JsonProperty("mimeType")]
        public readonly string MimeType;

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        [JsonProperty("name")]
        public readonly string FileName;

        /// <summary>
        /// Gets or sets a value indicating whether this version of document is signed.
        /// </summary>
        [JsonProperty("signed")]
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
                return this.Attributes.FirstOrDefault(x => x.Key == Constants.FileAttributes.TEMPLATE)?.Value;
            }
        }

        /// <summary>
        /// Gets value of attribute "GROUP" or null. Expected values: KOM1, SEN1, INV1.
        /// </summary>
        [JsonProperty("group")]
        public string Group
        {
            get
            {
                var group = this.Attributes.FirstOrDefault(x => x.Key == Constants.FileAttributes.GROUP)?.Value;

                if (string.IsNullOrEmpty(group))
                {
                    group = Constants.FileAttributeDefaults.GROUP;
                }

                return group;
            }
        }

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.GROUP_OBLIG"/> with value 'X'.
        /// </summary>
        [JsonProperty("obl_group")]
        public bool IsGroupOblig
        {
            get { return this.IsAttributeChecked(Constants.FileAttributes.GROUP_OBLIG); }
        }

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.PRINTED"/> with value 'X'.
        /// </summary>
        [JsonProperty("printed")]
        public bool IsPrinted
        {
            get { return this.IsAttributeChecked(Constants.FileAttributes.PRINTED); }
        }

        /// <summary>
        /// Gets <c>true</c> if there is <see cref="Constants.FileAttributes.SIGN_REQ"/> with value 'X'.
        /// </summary>
        [JsonProperty("sign")]
        public bool IsSignReq
        {
            get { return this.IsAttributeChecked(Constants.FileAttributes.SIGN_REQ); }
        }

        #endregion

        /// <summary>
        /// Prevents a default instance of the <see cref="OfferAttachmentXmlModel"/> class from being created.
        /// </summary>
        private OfferAttachmentXmlModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttachmentXmlModel"/> class.
        /// </summary>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="index">The index.</param>
        /// <param name="name">The name.</param>
        /// <param name="signedVersion">if set to <c>true</c> [signed version].</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="content">The content.</param>
        public OfferAttachmentXmlModel(string uniqueKey, string mimeType, string index, string name, bool signedVersion, OfferAttributeModel[] attributes, byte[] content)
        {
            this.UniqueKey = uniqueKey;
            this.MimeType = mimeType;
            this.Index = index;
            this.FileName = name;
            this.SignedVersion = signedVersion;
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
    }
}
