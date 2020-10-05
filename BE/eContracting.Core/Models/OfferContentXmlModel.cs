using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eContracting.Models
{
    [Serializable]
    public class OfferContentXmlModel
    {
        /// <summary>
        /// Gets or sets header.
        /// </summary>
        [XmlElement("Header")]
        public OfferHeaderXmlModel Header { get; set; }

        /// <summary>
        /// Gets or sets body.
        /// </summary>
        [XmlElement("Body")]
        public OfferBodyXmlModel Body { get; set; }
    }
}
