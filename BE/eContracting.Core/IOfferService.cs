using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Service with direct communication to SAP / CACHE.
    /// </summary>
    public interface IOfferService
    {
        /// <summary>
        /// Accepts the <paramref name="offer"/> synchronously. Calls <see cref="OFFER_TYPES.QUOTPRX_PRIJ"/> and set status to 5.
        /// </summary>
        /// <param name="offer">Valid offer.</param>
        /// <param name="data">The submitted data.</param>
        /// <param name="user">The user model.</param>
        /// <param name="sessionId">Current session id.</param>
        /// <returns>True if it was accepted or false.</returns>
        /// <exception cref="EcontractingApplicationException">When response from remote server was not success.</exception>
        void AcceptOffer(OfferModel offer, OfferSubmitDataModel data, UserCacheDataModel user, string sessionId);

        /// <summary>
        /// Reads offer synchronously.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">The user model.</param>
        /// <returns>True if it was set or false.</returns>
        /// <exception cref="EcontractingApplicationException">When response from remote server was not success.</exception>
        void ReadOffer(string guid, UserCacheDataModel user);

        /// <summary>
        /// Determinates if a user can read the offer.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">The user model.</param>
        /// <param name="type">Type of the offer.</param>
        /// <returns></returns>
        bool CanReadOffer(string guid, UserCacheDataModel user, OFFER_TYPES type);

        /// <summary>
        /// Gets the files synchronous.
        /// </summary>
        /// <param name="offer">The offer with attachment templates.</param>
        /// <param name="user">The user model.</param>
        /// <returns>Files in array or null</returns>
        OfferAttachmentModel[] GetAttachments(OffersModel offer, UserCacheDataModel user);

        /// <summary>
        /// Gets the files synchronous.
        /// </summary>
        /// <param name="offer">The offer with attachment templates.</param>
        /// <param name="user">The user model.</param>
        /// <returns>Files in array or null</returns>
        OfferAttachmentModel[] GetAttachments(OfferModel offer, UserCacheDataModel user);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> as <c>anonymous user</c> with text parameters synchronously.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <returns>The offer or null.</returns>
        OffersModel GetOffer(string guid);

        /// <summary>
        /// Gets collection of <see cref="OfferModel"/> initiated by given <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid">The initial guid.</param>
        /// <returns>Collection of the offers.</returns>
        ///OffersModel GetOffers(string guid);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> with text parameters synchronously.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">The user model.</param>
        /// <returns>The offer.</returns>
        /// <exception cref="AggregateException">When multiple issues happen in the process.</exception>
        OffersModel GetOffer(string guid, UserCacheDataModel user);

        /// <summary>
        /// Gets collection of <see cref="OfferModel"/> initiated by given <paramref name="guid"/> readable by <paramref name="user"/>.
        /// </summary>
        /// <param name="guid">The initial guid.</param>
        /// <param name="user">The user model.</param>
        /// <returns>Collection of the offers.</returns>
        ///OffersModel GetOffers(string guid, UserCacheDataModel user);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> asynchronously.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="includeTextParameters">When you need to get <see cref="OfferModel.TextParameters"/>.</param>
        /// <param name="user">The user model.</param>
        /// <returns>The offer.</returns>
        /// <exception cref="AggregateException">When multiple issues happen in the process.</exception>
        OffersModel GetOffer(string guid, UserCacheDataModel user, bool includeTextParameters);

        /// <summary>
        /// Gets collection of <see cref="OfferModel"/> initiated by given <paramref name="guid"/> readable by <paramref name="user"/>.
        /// </summary>
        /// <param name="guid">The initial guid.</param>
        /// <param name="user">The user model.</param>
        /// <param name="includeTextParameters">When you need to get <see cref="OfferModel.TextParameters"/>.</param>
        /// <returns>Collection of the offers.</returns>
        //OffersModel GetOffers(string guid, UserCacheDataModel user, bool includeTextParameters);

        OfferModel GetSingleOffer(string guid, UserCacheDataModel user, bool includeTextParameters);

        /// <summary>
        /// Make offer <paramref name="guid"/> signed.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">The user model.</param>
        /// <returns><c>True</c> if response was OK, otherwise <c>false</c>.</returns>
        /// <exception cref="EcontractingApplicationException">When response from remote server was not success.</exception>
        void SignInOffer(string guid, UserCacheDataModel user);

        /// <summary>
        /// Make offer <paramref name="guid"/> cancelled.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        void CancelOffer(string guid);
    }
}
