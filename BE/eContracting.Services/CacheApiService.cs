using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services.SAP;

namespace eContracting.Services
{
    /// <summary>
    /// Wrapper over <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class CacheApiService
    {
        private readonly ZCCH_CACHE_API Api;

        public CacheApiService()
        {
            this.Api = new ZCCH_CACHE_APIClient();
        }
    }
}
