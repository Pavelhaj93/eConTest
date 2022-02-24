using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;

namespace eContracting.Services
{
    public class RequestDataCacheService : IDataRequestCacheService
    {
        protected readonly ILogger Logger;

        public T Get<T>(string key)
        {
            var value = HttpContext.Current.Items[key];
            return (T)value;
        }

        public OfferCacheDataModel GetOffer(string guid)
        {
            var key = this.GetOfferCacheKey(guid);
            return this.Get<OfferCacheDataModel>(key);
        }

        public void Remove(string key)
        {
            HttpContext.Current.Items.Remove(key);
        }

        public void SaveOffer(string guid, OfferCacheDataModel data)
        {
            var key = this.GetOfferCacheKey(guid);
            this.Set(key, data);
        }

        public void Set<T>(string key, T data)
        {
            if (HttpContext.Current.Items.Contains(key))
            {
                this.Logger.Debug(null, $"Request cache already contains key '{key}'. Will be overwritten.");
            }

            HttpContext.Current.Items[key] = data;
        }

        private string GetOfferCacheKey(string guid)
        {
            return Constants.CacheKeys.OFFER_DATA + "." + guid;
        }
    }
}
