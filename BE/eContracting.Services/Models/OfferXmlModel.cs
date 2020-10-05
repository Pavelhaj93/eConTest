using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eContracting.Services.Models
{
    [Serializable]
    [XmlRoot("abap", Namespace = "http://www.sap.com/abapxml")]
    public class OfferXmlModel
    {
        /// <summary>
        /// Gets or sets inner content.
        /// </summary>
        [XmlElement("Nabidka", Namespace = "")]
        public OfferContentXmlModel Content { get; set; }
    }
}
