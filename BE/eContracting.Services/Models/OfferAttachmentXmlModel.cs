using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services.SAP;

namespace eContracting.Services.Models
{
    /// <summary>
    /// Represents file from 'ZCCH_CACHE_API'
    /// </summary>
    [Serializable]
    public class OfferAttachmentXmlModel
    {
        /// <summary>
        /// Gets or set index.
        /// </summary>
        public readonly string Index;

        /// <summary>
        /// Gets or sets file number.
        /// </summary>
        public readonly string FileNumber;

        /// <summary>
        /// Gets or sets a file type.
        /// </summary>
        public readonly string FileType;

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Gets or sets a value indicating whether signing is required.
        /// </summary>
        public readonly bool SignRequired;

        /// <summary>
        /// Gets or sets a template alc id.
        /// </summary>
        public readonly string TemplAlcId;

        /// <summary>
        /// Gets or sets a value indicating whether this version of document is signed.
        /// </summary>
        public readonly bool SignedVersion;

        /// <summary>
        /// Gets or sets file content.
        /// </summary>
        public readonly byte[] FileContent;

        public List<OfferFileAttribute> Attributes { get; } = new List<OfferFileAttribute>();

        /// <summary>
        /// Gets value of attribute "GROUP" or null. Expected values: KOM1, SEN1, INV1.
        /// </summary>
        public string Group
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == "GROUP")?.Value;
            }
        }

        /// <summary>
        /// Gets value of attribute "TEMPLATE" or null. Expected values: EES, A10, V01, V02, V03, EPO, AD1, ZPE, ED2, EP2X, DS1, DSE, V04, DP6, DP3.
        /// </summary>
        public string Template
        {
            get
            {
                return this.Attributes.FirstOrDefault(x => x.Key == "TEMPLATE")?.Value;
            }
        }

        public int Size
        {
            get
            {
                return this.FileContent?.Length ?? 0;
            }
        }

        public string SizeLabel
        {
            get
            {
                return Utils.GerReadableFileSize(this.Size);
            }
        }

        public OfferAttachmentXmlModel()
        {
        }

        public OfferAttachmentXmlModel(string index, string number, string type, string name, bool signRequired, string tempAlcId, bool signedVersion, byte[] content)
        {
            this.Index = index;
            this.FileNumber = number;
            this.FileType = type;
            this.FileName = name;
            this.SignRequired = signRequired;
            this.TemplAlcId = tempAlcId;
            this.SignedVersion = signedVersion;
            this.FileContent = content;
        }

        public bool HasAttribute(string key)
        {
            return this.Attributes.Any(x => x.Key == key);
        }
    }
}
