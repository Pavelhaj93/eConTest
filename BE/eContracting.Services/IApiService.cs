using System.Threading.Tasks;
using eContracting.Services.Models;

namespace eContracting.Services
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
        OfferModel GetOffer(string guid, string type);

        /// <summary>
        /// Gets the offer by <paramref name="guid"/> and <paramref name="type"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Offer type.</param>
        /// <returns>The offer.</returns>
        Task<OfferModel> GetOfferAsync(string guid, string type);

        /// <summary>
        /// Gets texts for offer <paramref name="guid"/> synchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>Array of texts.</returns>
        OfferTextModel[] GetXml(string guid);

        /// <summary>
        /// Gets texts for offer <paramref name="guid"/> asynchronously.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>Array of texts.</returns>
        Task<OfferTextModel[]> GetXmlAsync(string guid);

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
