using eContracting.Services.Models;

namespace eContracting.Services
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
    }
}
