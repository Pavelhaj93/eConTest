﻿using System;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Service with direct communication to SAP / CACHE.
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Accepts offer asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>True if it was accepted or false.</returns>
        Task<bool> AcceptOfferAsync(string guid);

        /// <summary>
        /// Accepts offer synchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>True if it was accepted or false.</returns>
        bool AcceptOffer(string guid);

        /// <summary>
        /// Reads offer synchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>True if it was set or false.</returns>
        bool ReadOffer(string guid);

        /// <summary>
        /// Reads offer asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>True if it was set or false.</returns>
        Task<bool> ReadOfferAsync(string guid);

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Files in array or null</returns>
        Task<OfferAttachmentXmlModel[]> GetAttachmentsAsync(string guid);

        /// <summary>
        /// Gets the files synchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Files in array or null</returns>
        OfferAttachmentXmlModel[] GetAttachments(string guid);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> and <paramref name="type"/> synchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Offer type.</param>
        /// <returns>The offer.</returns>
        /// <exception cref="AggregateException">When multiple issues happen in the process.</exception>
        OfferModel GetOffer(string guid, OFFER_TYPES type);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> and <paramref name="type"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Offer type.</param>
        /// <returns>The offer.</returns>
        /// <exception cref="AggregateException">When multiple issues happen in the process.</exception>
        Task<OfferModel> GetOfferAsync(string guid, OFFER_TYPES type);

        /// <summary>
        /// Make offer <paramref name="guid"/> signed.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns><c>True</c> if response was OK, otherwise <c>false</c>.</returns>
        bool SignInOffer(string guid);

        /// <summary>
        /// Make offer <paramref name="guid"/> signed asynchronously.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns><c>True</c> if response was OK, otherwise <c>false</c>.</returns>
        Task<bool> SignInOfferAsync(string guid);
    }
}
