using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Type of offers presented in SAP.
    /// </summary>
    public enum OFFER_TYPES
    {
        /// <summary>
        /// Type "QUOTPRX" contains basic information with textations.
        /// </summary>
        QUOTPRX,

        /// <summary>
        /// Legacy type "QUOTPRX_XML" contains only textations.
        /// </summary>
        QUOTPRX_XML,

        /// <summary>
        /// The "QUOTPRX_PDF" contains attached files for new offer.
        /// </summary>
        QUOTPRX_PDF,

        /// <summary>
        /// Type "NABIDKA_ARCH" contains attached files for accepted offer.
        /// </summary>
        QUOTPRX_ARCH,

        /// <summary>
        /// Type "NABIDKA_PRIJ" is used to accept an offer.
        /// </summary>
        QUOTPRX_PRIJ
    }
}
