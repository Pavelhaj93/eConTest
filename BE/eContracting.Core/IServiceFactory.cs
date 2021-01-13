using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;
using eContracting.SignStamp;

namespace eContracting
{
    public interface IServiceFactory
    {
        ZCCH_CACHE_APIClient CreateApi(CacheApiServiceOptions options);

        CRM_SIGN_STAMP_MERGEClient CreateApi(SignApiServiceOptions options);
    }
}
