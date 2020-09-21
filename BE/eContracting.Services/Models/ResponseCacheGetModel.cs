using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services.SAP;

namespace eContracting.Services.Models
{
    public class ResponseCacheGetModel
    {
        protected readonly ZCCH_CACHE_GETResponse Response;

        /// <summary>
        /// Determines whether the offer is accepted.
        /// </summary>
        public bool IsAccepted
        {
            get
            {
                return this.Response.ET_ATTRIB != null && this.Response.ET_ATTRIB.Any(x => x.ATTRID == "ACCEPTED_AT");
            }
        }

        public bool IsRetention
        {
            get
            {
                return false;
            }
        }

        public bool HasFiles
        {
            get
            {
                return this.Response.ET_FILES.Length > 0;
            }
        }

        public ResponseCacheGetModel(ZCCH_CACHE_GETResponse response)
        {
            this.Response = response;
        }
    }
}
