using System.Threading.Tasks;
using eContracting.Services.Models;

namespace eContracting.Services
{
    public interface IApiService
    {
        /// <summary>
        /// Accepts offer of <paramref name="guid"/> by setting <see cref="ZCCH_CACHE_STATUS_SET"/>.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>True if it was accepted or false.</returns>
        Task<bool> AcceptOfferAsync(string guid);

        /// <summary>
        /// Reads offer of <paramref name="guid"/> by setting <see cref="ZCCH_CACHE_STATUS_SET"/>.
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
        /// Gets the offer by <paramref name="guid"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <param name="type">Type from <see cref="AvailableRequestTypes"/> collection.</param>
        /// <returns></returns>
        Task<OfferModel> GetOfferAsync(string guid, string type);

        /// <summary>
        /// Gets texts for offer <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid">Guid identifier.</param>
        /// <returns>Array of texts.</returns>
        Task<OfferTextModel[]> GetXmlAsync(string guid);
    }
}
