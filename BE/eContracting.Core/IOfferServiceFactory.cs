using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting
{
    public interface IOfferServiceFactory
    {
        ZCCH_CACHE_API CreateCacheApi(CacheApiServiceOptions options);
    }
}
