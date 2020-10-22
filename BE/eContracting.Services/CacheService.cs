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
    public class CacheService : CustomCache, ICache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        public CacheService() : base("eContracting.Data", StringUtil.ParseSizeString("25MB"))
        {
        }

        /// <inheritdoc/>
        public void AddToRequest<T>(string key, T data)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Http context is empty");
            }

            HttpContext.Current.Items[key] = data;
        }

        /// <inheritdoc/>
        public void AddToSession<T>(string key, T data)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Http context is empty");
            }

            HttpContext.Current.Session[key] = data;
        }

        /// <inheritdoc/>
        public void AddToPersist<T>(string key, T data, TimeSpan interval)
        {
            this.InnerCache.Add(key, data, DateTime.UtcNow.Add(interval));
        }

        /// <inheritdoc/>
        public T GetFromRequest<T>(string key)
        {
            var data = HttpContext.Current?.Items?[key];

            if (data != null)
            {
                return (T)data;
            }

            return default(T);
        }

        /// <inheritdoc/>
        public T GetFromSession<T>(string key)
        {
            var data = HttpContext.Current?.Session?[key];

            if (data != null)
            {
                return (T)data;
            }

            return default(T);
        }

        /// <inheritdoc/>
        public T GetFromPersist<T>(string key)
        {
            if (this.InnerCache.ContainsKey(key))
            {
                var data = this.InnerCache[key];
                return (T)data;
            }

            return default(T);
        }
    }
}
