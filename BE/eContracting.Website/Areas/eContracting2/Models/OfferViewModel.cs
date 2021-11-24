using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class OfferViewModel : BaseAppConfigViewModel
    {
        [JsonIgnore]
        public StepsViewModel Steps { get; set; }

        [JsonIgnore]
        public string MainText { get; set; }

        public string GdprGuid { get; set; }

        public string GdprUrl { get; set; }

        public bool Debug { get; set; } = false;

        #region APIs

        [JsonProperty("offerUrl")]
        public readonly string OfferApi = "/api/econ/offer";

        [JsonProperty("acceptOfferUrl")]
        public readonly string OfferSubmitApi = "/api/econ/submit";

        [JsonProperty("getFileUrl")]
        public readonly string FileApi = "/api/econ/file";

        [JsonProperty("thumbnailUrl")]
        public readonly string FileImageApi = "/api/econ/thumbnail";

        [JsonProperty("signFileUrl")]
        public readonly string SignApi = "/api/econ/sign";

        [JsonProperty("uploadFileUrl")]
        public readonly string UploadFileApi = "/api/econ/upload";

        [JsonProperty("removeFileUrl")]
        public readonly string UploadRemoveFileUrl = "/api/econ/upload";

        [JsonProperty("keepAliveUrl")]
        public readonly string KeepAliveUrl = "/api/econ/keepalive";

        #endregion

        #region Urls

        [JsonProperty("thankYouPageUrl")]
        public string ThankYouPage { get; set; }

        [JsonProperty("sessionExpiredPageUrl")]
        public string SessionExpiredPage { get; set; }

        [JsonProperty("abMatrixCombinationPixelUrl")]
        public string AbMatrixCombinationPixelUrl { get; set; }

        #endregion

        [JsonProperty("allowedContentTypes")]
        public string[] AllowedContentTypes { get; set; }

        [JsonProperty("maxFileSize")]
        public int MaxFileSize { get; set; }

        [JsonProperty("maxGroupFileSize")]
        public int MaxGroupFileSize { get; set; }

        [JsonProperty("maxAllFilesSize")]
        public int MaxAllFilesSize { get; set; }

        public IDefinitionCombinationModel Definition { get; set; }

        [JsonProperty("suppliers")]
        public ListViewModel List { get; set; }

        public OfferViewModel(ISiteSettingsModel siteSettings) : base("Offer", siteSettings)
        {
        }
    }
}
