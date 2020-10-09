using System.Threading.Tasks;
using eContracting.Services.Models;

namespace eContracting.Services
{
    public interface IApiService
    {
        Task<bool> AcceptOfferAsync(string guid);
        Task<OfferAttachmentXmlModel[]> GetAttachmentsAsync(string guid);
        Task<OfferModel> GetOfferAsync(string guid, string type);
        Task<OfferTextModel[]> GetXmlAsync(string guid);
    }
}
