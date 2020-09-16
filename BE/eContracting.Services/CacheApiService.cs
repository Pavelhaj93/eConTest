using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.SAP;

namespace eContracting.Services
{
    /// <summary>
    /// Wrapper over <see cref="ZCCH_CACHE_API"/>.
    /// </summary>
    public class CacheApiService
    {
        public readonly CacheApiServiceOptions Options;

        private readonly ZCCH_CACHE_APIClient Api;

        public static string[] AvailableRequestTypes = new[] { "NABIDKA", "NABIDKA_XML", "NABIDKA_PDF", "NABIDKA_ARCH" };

        public CacheApiService(CacheApiServiceOptions options)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            this.Options = options;

            var binding = new BasicHttpBinding();
            binding.Name = "ZCCH_CACHE_APIClient";
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            var endpoint = new EndpointAddress(this.Options.Url);

            this.Api = new ZCCH_CACHE_APIClient(binding, endpoint);
            this.Api.ClientCredentials.UserName.UserName = this.Options.User;
            this.Api.ClientCredentials.UserName.Password = this.Options.Password;
        }

        /// <summary>
        /// Gets the files asynchronous.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Files in array or null</returns>
        public async Task<AttachmentModel[]> GetFilesAsync(string guid)
        {
            var result = await this.GetResponse(guid, "NABIDKA");

            if (result == null)
            {
                return null;
            }

            return null;
        }

        public async Task<ZCCH_CACHE_GETResponse> GetResponse(string guid, string type, string fileType = "B")
        {
            var get = new ZCCH_CACHE_GET();
            get.IV_CCHKEY = guid;
            get.IV_CCHTYPE = type;
            get.IV_GEFILE = fileType;

            var request = new ZCCH_CACHE_GETRequest(get);
            var response = await this.Api.ZCCH_CACHE_GETAsync(request);
            var result = response.ZCCH_CACHE_GETResponse;
            return result;
        }
    }
}
