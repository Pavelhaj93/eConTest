using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public interface IRespApiService
    {
        HttpResponseMessage GetResponse(HttpRequestMessage request);

        RestApiResponseModel<TResponse> GetResponse<TResponse>(HttpRequestMessage request);

        //HttpResponseMessage GetResponse(HttpMethod httpMethod, Uri endpoint, object jsonContent = null, NameValueCollection queryParameters = null);
    }
}
