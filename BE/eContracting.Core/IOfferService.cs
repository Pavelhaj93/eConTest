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
        /// Accepts offer synchronously.
        /// </summary>
        /// <param name="offer">Valid offer.</param>
        /// <param name="data">The submitted data.</param>
        /// <param name="sessionId">Current session id.</param>
        /// <returns>True if it was accepted or false.</returns>
        bool AcceptOffer(OfferModel offer, OfferSubmitDataModel data, string sessionId);

        /// <summary>
        /// Reads offer synchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>True if it was set or false.</returns>
        bool ReadOffer(string guid);

        /// <summary>
        /// Gets the files synchronous.
        /// </summary>
        /// <param name="offer">The offer with attachment templates.</param>
        /// <returns>Files in array or null</returns>
        OfferAttachmentModel[] GetAttachments(OfferModel offer);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> with text parameters synchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>The offer.</returns>
        /// <exception cref="AggregateException">When multiple issues happen in the process.</exception>
        OfferModel GetOffer(string guid);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="includeTextParameters">When you need to get <see cref="OfferModel.TextParameters"/>.</param>
        /// <returns>The offer.</returns>
        /// <exception cref="AggregateException">When multiple issues happen in the process.</exception>
        OfferModel GetOffer(string guid, bool includeTextParameters);

        /// <summary>
        /// Make offer <paramref name="guid"/> signed.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns><c>True</c> if response was OK, otherwise <c>false</c>.</returns>
        bool SignInOffer(string guid);
    }
}
