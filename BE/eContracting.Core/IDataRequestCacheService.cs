using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public interface IDataRequestCacheService : IDataCacheService
    {
        void SaveOffer(string guid, OfferCacheDataModel data);

        OfferCacheDataModel GetOffer(string guid);
    }
}
