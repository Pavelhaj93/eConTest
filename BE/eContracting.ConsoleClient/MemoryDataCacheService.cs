using eContracting.Models;

namespace eContracting.ConsoleClient
{
    // Used by console client - no cache needed 
    class MemoryDataCacheService : IRequestDataCacheService, IDataSessionCacheService
    {
        public T Get<T>(string key)
        {
            return default(T);            
        }

        public OfferModel GetOffer(string guid)
        {
            return this.Get<OfferModel>(guid);
        }

        public void Remove(string key)
        {
            // No cache needed for console client
        }

        public void SaveOffer(string guid, OfferModel data)
        {
            // No cache needed for console client
        }

        public void Set<T>(string key, T data)
        {
            // No cache needed for console client
        }
    }
}
