using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Represents file template from <see cref="OFFER_TYPES.QUOTPRX"/>.
    /// </summary>
    /// <remarks>This template doesn't contain a file content.</remarks>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OfferAttachmentXmlModel
    {
        /// <summary>
        /// Gets or sets sequence number across all files.
        /// </summary>
        /// <value>XML value <c>SEQNR</c>.</value>
        [XmlElement(Constants.FileAttributes.SEQUENCE_NUMBER)]
        [JsonIgnore]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets identifier of this template. It's not unique in every cases. If there are more attachment with the same value, uniqueness is compared with <see cref="Product"/>.
        /// </summary>
        /// <value>XML value <c>IDATTACH</c>.</value>
        [XmlElement(Constants.FileAttributes.TYPE)]
        [JsonProperty("idattach")]
        public string IdAttach { get; set; }

        /// <summary>
        /// Gets or sets template what file is based on.
        /// </summary>
        /// <value>XML value <c>TEMPLATE</c>.</value>
        [XmlElement(Constants.FileAttributes.TEMPLATE)]
        [JsonProperty("template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the description. In some cases it's used as file name.
        /// </summary>
        /// <value>XML value <c>DESCRIPTION</c>.</value>
        [XmlElement(Constants.FileAttributes.DESCRIPTION)]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets if this template is obligatory or not.
        /// </summary>
        /// <value>XML value <c>OBLIGATORY</c>. Contains <see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.OBLIGATORY)]
        [JsonProperty("obligatory")]
        public string Obligatory { get; set; }

        /// <summary>
        /// Gets or sets if real file exists or it's a template for upload.
        /// </summary>
        /// <value>XML value <c>PRINTED</c>. Contains <see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.PRINTED)]
        [JsonProperty("printed")]
        public string Printed { get; set; }

        /// <summary>
        /// Gets or sets if sing is required for this template.
        /// </summary>
        /// <value>XML value <c>SIGN_REQ</c>. Contains <see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.SIGN_REQ)]
        [JsonProperty("sing_required")]
        public string SignReq { get; set; }

        /// <summary>
        /// Gets or sets if timestamps is required or not.
        /// </summary>
        /// <value>XML value <c>TMST_REQ</c>. Contains <see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.TMST_REQ)]
        [JsonIgnore]
        public string TimeStampRequired { get; set; }

        /// <summary>
        /// Gets or sets additional info.
        /// </summary>
        /// <value>XML value <c>ADDINFO</c>.</value>
        [XmlElement(Constants.FileAttributes.ADDINFO)]
        [JsonIgnore]
        public string AddInfo { get; set; }

        /// <summary>
        /// Gets or sets some ALC id.
        /// </summary>
        /// <value>XML value <c>TEMPL_ALC_ID</c>.</value>
        [XmlElement(Constants.FileAttributes.TEMPL_ALC_ID)]
        [JsonIgnore]
        public string TemplAlcId { get; set; }

        /// <summary>
        /// Gets or sets additional identifier of this template. Value could be empty. It could be used with <see cref="IdAttach"/>.
        /// </summary>
        /// <value>XML value <c>PRODUCT</c>.</value>
        [XmlElement(Constants.FileAttributes.PRODUCT)]
        [JsonProperty("product")]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the group. Expected values: COMMODITY, DSL, NONCOMMODITY.
        /// </summary>
        /// <value>XML value <c>GROUP</c>.</value>
        [XmlElement(Constants.FileAttributes.GROUP)]
        [JsonProperty("group")]
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets templates with the same group are mandatory or not.
        /// </summary>
        /// <value>XML value <c>GROUP_OBLIGATORY</c>. Contains <see cref="Constants.FileAttributeValues.CHECK_VALUE"/> or empty.</value>
        [XmlElement(Constants.FileAttributes.GROUP_OBLIG)]
        [JsonProperty("group_obligatory")]
        public string GroupObligatory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets value to get compare text parameters for a group in accept dialog.
        /// </summary>
        /// <value>XML value <c>ITEM_GUID</c>.</value>
        [XmlElement(Constants.FileAttributes.ITEM_GUID)]
        [JsonIgnore]
        public string ItemGuid { get; set; }

        /// <summary>
        /// Gets or sets prefix for the document. Expecetd values: P or S.
        /// </summary>
        /// <value>XML value <c>CONSENT_TYPE</c>.</value>
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

        /// <summary>
        /// Gets all file template attributes as collection.
        /// </summary>
        public IDictionary<string, string> XmlTemplateAttributes
        {
            get
            {
                var dic = new Dictionary<string, string>();
                dic.Add(Constants.FileAttributes.SEQUENCE_NUMBER, this.SequenceNumber);
                dic.Add(Constants.FileAttributes.TYPE, this.IdAttach);
                dic.Add(Constants.FileAttributes.TEMPLATE, this.Template);
                dic.Add(Constants.FileAttributes.DESCRIPTION, this.Description);
                dic.Add(Constants.FileAttributes.OBLIGATORY, this.Obligatory);
                dic.Add(Constants.FileAttributes.PRINTED, this.Printed);
                dic.Add(Constants.FileAttributes.SIGN_REQ, this.SignReq);
                dic.Add(Constants.FileAttributes.TMST_REQ, this.TimeStampRequired);
                dic.Add(Constants.FileAttributes.ADDINFO, this.AddInfo);
                dic.Add(Constants.FileAttributes.TEMPL_ALC_ID, this.TemplAlcId);
                dic.Add(Constants.FileAttributes.PRODUCT, this.Product);
                dic.Add(Constants.FileAttributes.GROUP, this.Group);
                dic.Add(Constants.FileAttributes.GROUP_OBLIG, this.GroupObligatory);
                dic.Add(Constants.FileAttributes.ITEM_GUID, this.ItemGuid);
                dic.Add(Constants.FileAttributes.CONSENT_TYPE, this.ConsentType);
                return dic;
            }
        }

        /// <summary>
        /// Determines whether sign required or not for this file.
        /// </summary>
        public bool IsSignRequired()
        {
            return this.SignReq?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        /// <summary>
        /// Determines whether this file is printed or it's required for upload.
        /// </summary>
        public bool IsPrinted()
        {
            return this.Printed?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        /// <summary>
        /// Determines whether this file is obligatory.
        /// </summary>
        public bool IsObligatory()
        {
            return this.Obligatory?.Equals(Constants.FileAttributeValues.CHECK_VALUE, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        /// <summary>
        /// Determines whether group for this file obligatory.
        /// </summary>
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
