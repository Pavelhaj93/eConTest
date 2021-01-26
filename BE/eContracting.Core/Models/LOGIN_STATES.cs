using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Prerequisites for log-in procedure.
    /// </summary>
    public enum LOGIN_STATES
    {
        /// <summary>
        /// Everything is OK.
        /// </summary>
        OK,

        /// <summary>
        /// The offer not found by given guid.
        /// </summary>
        OFFER_NOT_FOUND,

        /// <summary>
        /// Provided guid identifier by user is invalid.
        /// </summary>
        INVALID_GUID,

        /// <summary>
        /// User is temporaly blocked due to max failed login attempts.
        /// </summary>
        USER_BLOCKED,

        /// <summary>
        /// The offer state in <see cref="OfferModel.State"/> equals to 1.
        /// </summary>
        OFFER_STATE_1,

        /// <summary>
        /// The missing birthday in <see cref="OfferModel.Birthday"/>.
        /// </summary>
        MISSING_BIRTHDAY
    }
}
