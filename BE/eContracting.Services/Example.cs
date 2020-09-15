using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Services
{
    class Example
    {
        public SAP.ZCCH_CACHE_API Api { get; set; }

        public async Task ABC()
        {
            var result = await Api.ZCCH_CACHE_GETAsync(null);
        }
    }
}
