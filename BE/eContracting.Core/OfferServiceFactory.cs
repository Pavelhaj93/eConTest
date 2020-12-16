using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting
{
    public class OfferServiceFactory : IOfferServiceFactory
    {
        public ZCCH_CACHE_API CreateCacheApi(CacheApiServiceOptions options)
        {
            var binding = new BasicHttpBinding();
            binding.Name = nameof(ZCCH_CACHE_APIClient);
            binding.MaxReceivedMessageSize = 65536 * 100; // this is necessary for "NABIDKA_PDF"
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            var endpoint = new EndpointAddress(options.Url);

            var api = new ZCCH_CACHE_APIClient(binding, endpoint);
            api.ClientCredentials.UserName.UserName = options.User;
            api.ClientCredentials.UserName.Password = options.Password;
            return api;
        }
    }
}
