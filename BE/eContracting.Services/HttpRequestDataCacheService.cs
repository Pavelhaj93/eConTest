using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;

namespace eContracting.Services
{
    [ExcludeFromCodeCoverage]
    public class HttpRequestDataCacheService : IRequestDataCacheService
    {
        protected readonly ILogger Logger;
        protected const string CACHE_GUID = "REQUEST_CACHE";
        public HttpRequestDataCacheService(ILogger logger)
        {
            this.Logger = logger;
        }

        public T Get<T>(string key)
        {
            key = this.GetEconKey(key);

            if (HttpContext.Current.Items.Contains(key))
            {
                var value = HttpContext.Current.Items[key];
                this.Logger.Debug(CACHE_GUID, $"Get data by key = '{key}'");
                return (T)value;
            }

            return default(T);
        }

        public OfferModel GetOffer(string guid)
        {
            return this.Get<OfferModel>(guid);
        }

        public void Remove(string key)
        {
            if (HttpContext.Current.Items.Contains(key))
            {
                HttpContext.Current.Items.Remove(key);
                this.Logger.Debug(CACHE_GUID, $"Remove data by key = '{key}'");
            }
        }

        public void SaveOffer(string guid, OfferModel data)
        {
            var key = this.GetEconKey(guid);
            this.Set(key, data);
        }

        public void Set<T>(string key, T data)
        {
            key = this.GetEconKey(key);

            if (HttpContext.Current.Items.Contains(key))
            {
                this.Logger.Debug(CACHE_GUID, $"cache already contains data under key '{key}'. Will be overwritten.");
            }

            HttpContext.Current.Items[key] = data;
            this.Logger.Debug(CACHE_GUID, $"Save data under key = '{key}'");
        }

        private string GetEconKey(string key)
        {
            return Constants.CacheKeys.OFFER_DATA + "&" + key;
        }
    }
}
