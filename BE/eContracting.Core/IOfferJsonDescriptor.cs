using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Creates prescription for JSON / API for not accepted offer and accepted offer.
    /// </summary>
    public interface IOfferJsonDescriptor
    {
        /// <summary>
        /// Gets prescription for not accepted offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="user">The user.</param>
        JsonOfferNotAcceptedModel GetNew(OfferModel offer, UserCacheDataModel user);

        /// <summary>
        /// Gets prescription for accepted offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// /// <param name="user">The user.</param>
        /// <returns></returns>
        JsonOfferAcceptedModel GetAccepted(OfferModel offer, UserCacheDataModel user);
    }
}
