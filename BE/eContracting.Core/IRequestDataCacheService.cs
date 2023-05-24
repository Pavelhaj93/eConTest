using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Cache service using HTTP
    /// </summary>
    public interface IRequestDataCacheService : IDataCacheService
    {
        void SaveOffer(string key, OfferModel data);

        OfferModel GetOffer(string key);
    }
}
