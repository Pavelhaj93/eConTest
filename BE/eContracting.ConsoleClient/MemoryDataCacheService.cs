using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.ConsoleClient
{
    class MemoryDataCacheService : IDataRequestCacheService, IDataSessionCacheService
    {
        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public OfferCacheDataModel GetOffer(string guid)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void SaveOffer(string guid, OfferCacheDataModel data)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T data)
        {
            throw new NotImplementedException();
        }
    }
}
