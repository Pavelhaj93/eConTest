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
        /// Type "NABIDKA" contains basic information with textations.
        /// </summary>
        NABIDKA,

        /// <summary>
        /// Legacy type "NABIDKA_XML" contains only textations.
        /// </summary>
        NABIDKA_XML,

        /// <summary>
        /// The "NABIDKA_PDF" contains attached files for new offer.
        /// </summary>
        NABIDKA_PDF,

        /// <summary>
        /// Type "NABIDKA_ARCH" contains attached files for accepted offer.
        /// </summary>
        NABIDKA_ARCH
    }
}
