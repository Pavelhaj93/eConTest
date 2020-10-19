using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.Services
{
    /// <summary>
    /// Extensions for <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public static class ZCCH_CACHE_API_Extensions
    {
        /// <summary>
        /// Checks if the response contains files.
        /// </summary>
        /// <param name="res">Response from SAP.</param>
        /// <returns>Retruns tru if response contains files.</returns>
        public static bool ThereAreFiles(this ZCCH_CACHE_GETResponse res)
        {
            return res.ET_FILES?.Length > 0;
        }
    }
}
