using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eContracting.Models
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    [XmlRoot("abap", Namespace = "http://www.sap.com/abapxml")]
    public class OfferXmlModel
    {
        /// <summary>
        /// Gets or sets inner content.
        /// </summary>
        [XmlElement("Nabidka", Namespace = "")]
        public OfferContentXmlModel Content { get; set; }

        [XmlIgnore]
        public string RawXml { get; set; }
    }
}
