using System.Collections.Generic;
using eContracting.Models;
using eContracting.Services;

namespace eContracting
{
    /// <summary>
    /// Represents parser from <see cref="OfferModel"/> to <see cref="ResponseCacheGetModel"/> object.
    /// </summary>
    public interface IOfferParserService
    {
        /// <summary>
        /// Generates offer.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Return <see cref="OfferModel"/> or null if there are no files in <see cref="ResponseCacheGetModel"/>.</returns>
        OfferModel GenerateOffer(ResponseCacheGetModel response);

        /// <summary>
        /// Gets offer version from <paramref name="response"/> data.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>Numbered version starting at 1.</returns>
        int GetVersion(ZCCH_CACHE_GETResponse response);

        /// <summary>
        /// Gets the text parameters from given <paramref name="files"/>.
        /// </summary>
        /// <remarks>There files needs to have XML path 'form/parameters'.</remarks>
        /// <param name="files">The files.</param>
        /// <returns>Dictionary or null.</returns>
        IDictionary<string, string> GetTextParameters(ZCCH_ST_FILE[] files);

        /// <summary>
        /// Makes <paramref name="textParameters"/> compatible in respect to offer <paramref name="version"/>.
        /// </summary>
        /// <param name="textParameters">The text parameters.</param>
        /// <param name="version">Offer version.</param>
        void MakeCompatibleParameters(IDictionary<string, string> textParameters, int version);
    }
}
