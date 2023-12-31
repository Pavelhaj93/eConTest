﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;
using eContracting.SignStamp;

namespace eContracting
{
    /// <inheritdoc/>
    /// <seealso cref="eContracting.IServiceFactory" />
    public class ServiceFactory : IServiceFactory
    {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public CRM_SIGN_STAMP_MERGEClient CreateApi(SignApiServiceOptions options)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var binding = this.GetBinding(options, nameof(CRM_SIGN_STAMP_MERGEClient));
            var endpoint = new EndpointAddress(options.Url);
            
            var api = new CRM_SIGN_STAMP_MERGEClient(binding, endpoint);
            //api.ClientCredentials.HttpDigest.ClientCredential = new NetworkCredential(options.User, options.Password);
            api.ClientCredentials.UserName.UserName = options.User;
            api.ClientCredentials.UserName.Password = options.Password;
            return api;
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public ZCCH_CACHE_APIClient CreateApi(CacheApiServiceOptions options)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var binding = this.GetBinding(options, nameof(ZCCH_CACHE_APIClient));
            var endpoint = new EndpointAddress(options.Url);

            var api = new ZCCH_CACHE_APIClient(binding, endpoint);
            api.ClientCredentials.UserName.UserName = options.User;
            api.ClientCredentials.UserName.Password = options.Password;
            return api;
        }

        /// <summary>
        /// Gets correct binding based on <see cref="BaseApiServiceOptions.Url"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        /// <returns>For <c>https</c> <see cref="BasicHttpsBinding"/>, for <c>http</c> <see cref="BasicHttpBinding"/>.</returns>
        protected internal Binding GetBinding(BaseApiServiceOptions options, string name)
        {
            if (options.IsHttps)
            {
                var binding = new BasicHttpsBinding();
                binding.Name = name;
                binding.MaxReceivedMessageSize = 13107200; // this is necessary for "NABIDKA_PDF"
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                return binding;
            }
            else
            {
                var binding = new BasicHttpBinding();
                binding.Name = name;
                binding.MaxReceivedMessageSize = 13107200; // this is necessary for "NABIDKA_PDF"
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                return binding;
            }
        }
    }
}
