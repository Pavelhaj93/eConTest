﻿using System;
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
    public class CacheService : CustomCache, IUserCacheService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        public CacheService() : base("eContracting.Data", StringUtil.ParseSizeString("25MB"))
        {
        }

        /// <inheritdoc/>
        public void Add<T>(string key, T data)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Http context is empty");
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
    }
}
