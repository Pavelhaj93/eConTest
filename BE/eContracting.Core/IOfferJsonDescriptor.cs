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
        Task<JsonOfferNotAcceptedModel> GetNewAsync(OfferModel offer);

        /// <summary>
        /// Gets prescription for accepted offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns></returns>
        Task<JsonOfferAcceptedModel> GetAcceptedAsync(OfferModel offer);
    }
}
