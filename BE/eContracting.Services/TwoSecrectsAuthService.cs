using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using eContracting.Models;

namespace eContracting.Services
{
    /// <inheritdoc/>
    public class TwoSecrectsAuthService : ITwoSecrectsAuthService
    {
        public AUTH_METHODS AuthType { get; } = AUTH_METHODS.TWO_SECRETS;

        /// <summary>
        /// The settings reader.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReader;

        /// <summary>
        /// The cache.
        /// </summary>
        protected readonly IDataSessionCacheService Cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoSecrectsAuthService"/> class.
        /// </summary>
        /// <param name="settingsReader">The settings reader.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <exception cref="ArgumentNullException">
        /// settingsReader
        /// or
        /// cacheService
        /// </exception>
        public TwoSecrectsAuthService(ISettingsReaderService settingsReader, IDataSessionCacheService cacheService)
        {
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
            this.Cache = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <inheritdoc/>
        public void Authenticate(UserCacheDataModel authData)
        {
            this.Cache.Set(Constants.CacheKeys.USER_DATA, authData);
        }

        /// <inheritdoc/>
        public bool IsAuthenticated(HttpRequestBase request)
        {
            return this.GetCurrentUser() != null;
        }

        /// <inheritdoc/>
        public bool IsAuthenticated(HttpRequestContext request)
        {
            return this.GetCurrentUser() != null;
        }

        /// <inheritdoc/>
        public UserCacheDataModel GetCurrentUser()
        {
            return this.Cache.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA);
        }
    }
}
