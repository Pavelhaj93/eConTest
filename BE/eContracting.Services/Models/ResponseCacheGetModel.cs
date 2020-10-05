using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using eContracting.Models;
using eContracting.Services.SAP;

namespace eContracting.Services.Models
{
    public class ResponseCacheGetModel
    {
        public readonly ZCCH_CACHE_GETResponse Response;

        public bool HasFiles
        {
            get
            {
                return this.Response.ET_FILES.Length > 0;
            }
        }

        public AttachmentModel[] Attachments { get; set; }

        public ResponseCacheGetModel(ZCCH_CACHE_GETResponse response)
        {
            this.Response = response;
        }
    }
}
