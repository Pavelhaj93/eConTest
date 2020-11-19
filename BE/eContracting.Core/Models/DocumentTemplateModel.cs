using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

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
        public string SequenceNumber { get; set; }

        [XmlElement("IDATTACH")]
        public string IdAttach { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; }

        [XmlElement("OBLIGATORY")]
        public string Obligatory { get; set; }

        [XmlElement("PRINTED")]
        public string Printed { get; set; }

        [XmlElement("SIGN_REQ")]
        public string SignReq { get; set; }

        [XmlElement("TMST_REQ")]
        public string TimeStampRequired { get; set; }

        [XmlElement("ADDINFO")]
        public string AddInfo { get; set; }

        [XmlElement("TEMPL_ALC_ID")]
        public string TemplAlcId { get; set; }

        [XmlElement("GROUP")]
        public string Group { get; set; }

        [XmlElement("GROUP_OBLIGATORY")]
        public string GroupObligatory { get; set; } = string.Empty;

        [XmlElement("ITEM_GUID")]
        public string ItemGuid { get; set; }
    }
}
