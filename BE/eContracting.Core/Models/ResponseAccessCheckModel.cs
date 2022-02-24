using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.Models
{
    [ExcludeFromCodeCoverage]
    public class ResponseAccessCheckModel
    {
        /// <summary>
        /// Raw response from SAP.
        /// </summary>
        public readonly ZCCH_CACHE_ACCESS_CHECKResponse Response;

        public ResponseAccessCheckModel(ZCCH_CACHE_ACCESS_CHECKResponse response)
        {
            this.Response = response ?? throw new ArgumentNullException(nameof(response));
        }
    }
}
