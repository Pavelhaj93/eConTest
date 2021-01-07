using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Represents document template from <see cref="OFFER_TYPES.NABIDKA"/>.
    /// </summary>
    /// <remarks>This template doesn't contain a file content.</remarks>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DocumentTemplateModel
    {
        [XmlElement(Constants.FileAttributes.SEQUENCE_NUMBER)]
        [JsonIgnore]
        public string SequenceNumber { get; set; }

        [XmlElement(Constants.FileAttributes.TYPE)]
        [JsonProperty("idattach")]
        public string IdAttach { get; set; }

        [XmlElement(Constants.FileAttributes.DESCRIPTION)]
        [JsonProperty("description")]
        public string Description { get; set; }

        [XmlElement(Constants.FileAttributes.OBLIGATORY)]
        [JsonProperty("obligatory")]
        public string Obligatory { get; set; }

        [XmlElement(Constants.FileAttributes.PRINTED)]
        [JsonProperty("printed")]
        public string Printed { get; set; }

        [XmlElement(Constants.FileAttributes.SIGN_REQ)]
        [JsonProperty("sing_required")]
        public string SignReq { get; set; }

        [XmlElement(Constants.FileAttributes.TMST_REQ)]
        [JsonIgnore]
        public string TimeStampRequired { get; set; }

        [XmlElement(Constants.FileAttributes.ADDINFO)]
        [JsonIgnore]
        public string AddInfo { get; set; }

        [XmlElement(Constants.FileAttributes.TEMPL_ALC_ID)]
        [JsonIgnore]
        public string TemplAlcId { get; set; }

        [XmlElement(Constants.FileAttributes.GROUP)]
        [JsonProperty("group")]
        public string Group { get; set; }

        [XmlElement(Constants.FileAttributes.GROUP_OBLIG)]
        [JsonIgnore]
        public string GroupObligatory { get; set; } = string.Empty;

        [XmlElement(Constants.FileAttributes.ITEM_GUID)]
        [JsonIgnore]
        public string ItemGuid { get; set; }

        /// <summary>
        /// Gets or sets prefix for the document.
        /// </summary>
        [XmlElement(Constants.FileAttributes.CONSENT_TYPE)]
        [JsonIgnore]
        public string ConsentType { get; set; }

        /// <summary>
        /// Gets the unique key for this template.
        /// </summary>
        [JsonIgnore]
        public string UniqueKey
        {
            get
            {
                return Utils.GetUniqueKey(this);
            }
        }

        public bool IsSignRequired()
        {
            return this.SignReq?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool IsPrinted()
        {
            return this.Printed?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool IsObligatory()
        {
            return this.Obligatory?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool IsGroupObligatory()
        {
            return this.GroupObligatory?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> with <see cref="IdAttach"/> and <see cref="Description"/>.
        /// </returns>
        public override string ToString()
        {
            return this.IdAttach + " " + this.Description;
        }
    }
}
