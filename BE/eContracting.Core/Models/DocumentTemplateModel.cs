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

        /// <summary>
        /// Gets or sets identifier of this template. It's not unique in every cases. If there are more attachment with the same value, uniqueness is compared with <see cref="Product"/>.
        /// </summary>
        /// <value>Cannot be empty.</value>
        [XmlElement(Constants.FileAttributes.TYPE)]
        [JsonProperty("idattach")]
        public string IdAttach { get; set; }

        /// <summary>
        /// Gets or sets additional identifier of this template. Value could be empty. It could be used with <see cref="IdAttach"/>.
        /// </summary>
        /// <value>Could be empty.</value>
        [XmlElement(Constants.FileAttributes.PRODUCT)]
        [JsonProperty("product")]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the description. In some cases it's used as file name.
        /// </summary>
        [XmlElement(Constants.FileAttributes.DESCRIPTION)]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets if this template is obligatory or not.
        /// </summary>
        /// <value><see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.OBLIGATORY)]
        [JsonProperty("obligatory")]
        public string Obligatory { get; set; }

        /// <summary>
        /// Gets or sets if real file exists or it's a template for upload.
        /// </summary>
        /// <value><see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.PRINTED)]
        [JsonProperty("printed")]
        public string Printed { get; set; }

        /// <summary>
        /// Gets or sets sing is required for this template.
        /// </summary>
        /// <value><see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
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

        /// <summary>
        /// Gets or sets the group. Expected values: COMMODITY, DSL, NONCOMMODITY.
        /// </summary>
        /// <value>Shouldn't be null.</value>
        [XmlElement(Constants.FileAttributes.GROUP)]
        [JsonProperty("group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets templates with the same group are mandatory or not.
        /// </summary>
        /// <value><see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.GROUP_OBLIG)]
        [JsonProperty("group_obligatory")]
        public string GroupObligatory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets value to get compare text parameters for a group in accept dialog.
        /// </summary>
        /// <value>
        /// The item unique identifier.
        /// </value>
        [XmlElement(Constants.FileAttributes.ITEM_GUID)]
        [JsonIgnore]
        public string ItemGuid { get; set; }

        /// <summary>
        /// Gets or sets prefix for the document. Expecetd values: P or S.
        /// </summary>
        /// <value>Shouldn't be empty.</value>
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
