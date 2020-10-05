using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class OfferModel
    {
        
        public OfferXmlModel Xml { get; set; }

        /// <summary>
        /// Gets or sets flag if offer is accepted.
        /// </summary>
        public bool IsAccepted { get; set; }

        /// <summary>
        /// Gets or sets infromation when offer was accepted.
        /// </summary>
        public string AcceptedAt { get; set; }

        /// <summary>
        /// Gets or sets CREATED_AT value.
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets information if offer has GDPR.
        /// </summary>
        public bool HasGDPR { get; set; }

        /// <summary>
        /// Gets or sets GDPRKey.
        /// </summary>
        public string GDPRKey { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        public string State { get; set; }

        public OfferModel(OfferXmlModel xml)
        {
            this.Xml = xml;
        }
    }
}
