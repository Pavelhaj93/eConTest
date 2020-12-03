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
    public class UserSessionDataCacheService : IUserDataCacheService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionDataCacheService"/> class.
        /// </summary>
        public UserSessionDataCacheService()
        {
        }

        /// <inheritdoc/>
        public void Set<T>(string key, T data)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Http context is not available");
            }

            if (HttpContext.Current.Session == null)
            {
                throw new InvalidOperationException("Session is not available");
            }

            HttpContext.Current.Session[key] = data;
        }

        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            var data = HttpContext.Current?.Session?[key];

            if (data != null)
            {
                return (T)data;
            }

            return default(T);
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            HttpContext.Current?.Session?.Remove(key);
            //TODO: Remove data related to session
        }
    }
}
