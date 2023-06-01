using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Models.JsonDescriptor;

namespace eContracting
{
    /// <summary>
    /// Creates prescription for JSON / API for not accepted offer and accepted offer.
    /// </summary>
    public interface IOfferJsonDescriptor
    {
        /// <summary>
        /// Gets summary infor for an offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        JsonOfferSummaryModel GetSummary(OffersModel offer, UserCacheDataModel user);

        /// <summary>
        /// Gets summary infor for an offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>        
        ContainerModel GetSummary2(OffersModel offer, UserCacheDataModel user);

        /// <sum
        /// <summary>
        /// Gets prescription for not accepted offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="user">The user.</param>
        JsonOfferNotAcceptedModel GetNew(OffersModel offer, UserCacheDataModel user);
        ContainerModel GetNew2(OffersModel offer, UserCacheDataModel user);

        /// <summary>
        /// Gets prescription for accepted <paramref name="offers"/>.
        /// </summary>
        /// <param name="offers">Collection of accepted offers.</param>
        /// <param name="user">The user.</param>
        JsonOfferAcceptedModel GetAccepted(OffersModel offers, UserCacheDataModel user);
    }
}

