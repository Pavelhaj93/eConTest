using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services;

namespace eContracting.Models
{
    [ExcludeFromCodeCoverage]
    public class ResponseStatusSetModel
    {
        public readonly ZCCH_CACHE_STATUS_SETResponse1 Response;

        public int ErrorCode => this.Response.ZCCH_CACHE_STATUS_SETResponse.EV_RETCODE;

        public ResponseStatusSetModel(ZCCH_CACHE_STATUS_SETResponse1 response)
        {
            this.Response = response;
        }

        public ErrorModel GetError(string code)
        {
            return ERROR_CODES.FromResponse(code, this.Response.ZCCH_CACHE_STATUS_SETResponse);
        }
    }
}
