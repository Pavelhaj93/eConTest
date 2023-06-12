using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class UploadViewModel : BaseAppConfigViewModel
    {
        [JsonIgnore]
        public StepsViewModel Steps { get; set; }

        [JsonIgnore]
        public string MainText { get; set; }

        [JsonProperty("uploadUrl")]
        public readonly string UploadApi = "/api/econ/uploads";

        [JsonProperty("uploadFileUrl")]
        public readonly string UploadFileApi = "/api/econ/upload";

        [JsonProperty("removeFileUrl")]
        public readonly string UploadRemoveFileUrl = "/api/econ/upload";

        [JsonProperty("getSummaryUrl")]
        public readonly string SummaryApi = "/api/econ/summary";

        [JsonProperty("allowedContentTypes")]
        public string[] AllowedContentTypes { get; set; }

        [JsonProperty("maxFileSize")]
        public int MaxFileSize { get; set; }

        [JsonProperty("maxGroupFileSize")]
        public int MaxGroupFileSize { get; set; }

        [JsonProperty("maxAllFilesSize")]
        public int MaxAllFilesSize { get; set; }



        [JsonProperty("keepAliveUrl")]
        public readonly string KeepAliveUrl = "/api/econ/keepalive";

        [JsonProperty("getCallMeBackUrl")]
        public string CmbGetUrl { get; set; } = "/api/econ/getscmb";

        [JsonProperty("postCallMeBackUrl")]
        public string CmbPostUrl { get; set; } = "/api/econ/sendscmb";

        [JsonProperty("offerUrl")]
        public string OfferPage { get; set; }

        [JsonProperty("authUrl")]
        public string LoginPage { get; set; }

        [JsonProperty("nextUrl")]
        public string NextUrl { get; set; }

        [JsonProperty("backUrl")]
        public string BackUrl { get; set;  }

        [JsonIgnore]
        public IDefinitionCombinationModel Definition { get; set; }

        public UploadViewModel(ISiteSettingsModel siteSettings, string guid) : base("Upload", siteSettings, guid)
        {
        }
    }
}
