using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Services
{
    /// <summary>
    /// Represents REST API client to use only one instance of <see cref="HttpClient"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GeneralRestApiService : IRespApiService
    {
        protected static readonly HttpClient Client = new HttpClient();
        protected static CancellationTokenSource TokenSource = new CancellationTokenSource();
        protected readonly ILogger Logger;

        public GeneralRestApiService(ILogger logger)
        {
            this.Logger = logger;
        }

        static GeneralRestApiService()
        {
            //Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpResponseMessage GetResponse(HttpRequestMessage request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };

            return Client.SendAsync(request).Result;
        }

        public RestApiResponseModel<TResponse> GetResponse<TResponse>(HttpRequestMessage request)
        {
            this.Logger.Debug(null, "Request started: " + request.ToString());
                        
            var ct = TokenSource.Token;

            var stop = new Stopwatch();
            stop.Start();
            var response = this.GetResponse(request);
            stop.Stop();

            this.Logger.Debug(null, "Request finished: " + response?.ToString() ?? "null");

            if (response == null)
            {
                return null;
            }

            TResponse data = default(TResponse);

            if (!response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                this.Logger.Error(null, $"Request to '{request.RequestUri.ToString()}' returned error ({response.StatusCode}): " + content);
                return new RestApiResponseModel<TResponse>(default(TResponse), response.StatusCode, response.IsSuccessStatusCode, content);
            }

            if (typeof(TResponse) == typeof(byte[]))
            {
                data = (TResponse)(object)(response.Content.ReadAsByteArrayAsync().Result);
            }
            else if (typeof(TResponse) == typeof(Stream))
            {
                data = (TResponse)(object)(response.Content.ReadAsStreamAsync().Result);
            }
            else if (typeof(TResponse) != typeof(string))
            {
                var str = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<TResponse>(str);
                data = (TResponse)obj;
            }
            else
            {
                var str = response.Content.ReadAsStringAsync().Result;
                data = (TResponse)(object)str;
            }
            
            var model = new RestApiResponseModel<TResponse>(data, response.StatusCode, response.IsSuccessStatusCode);
            return model;
        }
    }
}
