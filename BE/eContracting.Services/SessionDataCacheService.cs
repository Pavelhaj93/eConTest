using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore;
using Sitecore.Caching;

namespace eContracting.Services
{
    /// <inheritdoc/>
    public class SessionDataCacheService : IDataSessionCacheService
    {
        /// <summary>
        /// The session provider.
        /// </summary>
        protected readonly ISessionProvider SessionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionDataCacheService"/> class.
        /// </summary>
        public SessionDataCacheService(ISessionProvider sessionProvider)
        {
            this.SessionProvider = sessionProvider;
        }

        /// <inheritdoc/>
        public void Set<T>(string key, T data)
        {
            this.SessionProvider.Set(key, data, true);
        }

        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            return this.SessionProvider.GetValue<T>(key);
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            this.SessionProvider.Remove(key);
        }
    }
}
