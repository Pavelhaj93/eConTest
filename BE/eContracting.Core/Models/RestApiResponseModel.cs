using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class RestApiResponseModel<TData>
    {
        public readonly HttpStatusCode StatusCode;

        public readonly bool IsSuccessStatusCode;

        public readonly TData Data;

        public readonly string Error;

        public RestApiResponseModel(TData data, HttpStatusCode statusCode, bool isSuccessStatusCode, string error = null)
        {
            this.Data = data;
            this.StatusCode = statusCode;
            this.IsSuccessStatusCode = isSuccessStatusCode;
            this.Error = error;
        }
    }
}
