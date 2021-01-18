using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;
using eContracting.SignStamp;

namespace eContracting
{
    /// <summary>
    /// Factory to create correct instances of the services based on input parameters.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Creates API for <see cref="ZCCH_CACHE_APIClient"/> based on input <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Instance of API client.</returns>
        ZCCH_CACHE_APIClient CreateApi(CacheApiServiceOptions options);

        /// Creates API for <see cref="CRM_SIGN_STAMP_MERGEClient"/> based on input <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Instance of API client.</returns>
        CRM_SIGN_STAMP_MERGEClient CreateApi(SignApiServiceOptions options);
    }
}
