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

        (Dictionary<string, string> parameters, string rawContent) GetTextParameters(ZCCH_ST_FILE file);
    }
}
