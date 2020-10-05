using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eContracting.Models
{
    [Serializable()]
    [XmlRoot("abap", Namespace = "http://www.sap.com/abapxml")]
    public class DocumentWrapperModel
    {
        /// <summary>
        /// Gets or sets internal offer.
        /// </summary>
        [XmlElement("Nabidka", Namespace = "")]
        public DocumentModel OfferInternal { get; set; } //TODO: Rename to 'Document'
    }
}
