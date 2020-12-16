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
        [XmlElement("SEQNR")]
        [JsonIgnore]
        public string SequenceNumber { get; set; }

        [XmlElement("IDATTACH")]
        [JsonProperty("idattach")]
        public string IdAttach { get; set; }

        [XmlElement("DESCRIPTION")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [XmlElement("OBLIGATORY")]
        [JsonProperty("obligatory")]
        public string Obligatory { get; set; }

        [XmlElement("PRINTED")]
        [JsonProperty("printed")]
        public string Printed { get; set; }

        [XmlElement("SIGN_REQ")]
        [JsonProperty("sing_required")]
        public string SignReq { get; set; }

        [XmlElement("TMST_REQ")]
        [JsonIgnore]
        public string TimeStampRequired { get; set; }

        [XmlElement("ADDINFO")]
        [JsonIgnore]
        public string AddInfo { get; set; }

        [XmlElement("TEMPL_ALC_ID")]
        [JsonIgnore]
        public string TemplAlcId { get; set; }

        [XmlElement("GROUP")]
        [JsonProperty("group")]
        public string Group { get; set; }

        [XmlElement("GROUP_OBLIGATORY")]
        [JsonIgnore]
        public string GroupObligatory { get; set; } = string.Empty;

        [XmlElement("ITEM_GUID")]
        [JsonIgnore]
        public string ItemGuid { get; set; }

        /// <summary>
        /// Gets or sets prefix for the document.
        /// </summary>
        [XmlElement("CONSENT_TYPE")]
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
