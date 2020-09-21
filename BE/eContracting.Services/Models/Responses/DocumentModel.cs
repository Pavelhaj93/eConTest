using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eContracting.Services.Models.Responses
{
    [Serializable]
    public class DocumentModel
    {
        /// <summary>
        /// Gets or sets header.
        /// </summary>
        [XmlElement("Header")]
        public DocumentHeaderModel Header { get; set; }

        /// <summary>
        /// Gets or sets body.
        /// </summary>
        [XmlElement("Body")]
        public DocumentBodyModel Body { get; set; }

        /// <summary>
        /// Gets or sets flag if offer is accepted.
        /// </summary>
        [XmlIgnore]
        public Boolean IsAccepted { get; set; }

        /// <summary>
        /// Gets or sets infromation when offer was accepted.
        /// </summary>
        [XmlIgnore]
        public string AcceptedAt { get; set; }

        /// <summary>
        /// Gets or sets CREATED_AT value.
        /// </summary>
        [XmlIgnore]
        public string CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets information if offer has GDPR.
        /// </summary>
        [XmlIgnore]
        public bool HasGDPR { get; set; }

        /// <summary>
        /// Gets or sets GDPRKey.
        /// </summary>
        [XmlIgnore]
        public string GDPRKey { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [XmlIgnore]
        public string State { get; set; }
    }
}
