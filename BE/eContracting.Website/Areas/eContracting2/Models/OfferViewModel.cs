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
        public string MainText { get; set; }

        public string GdprGuid { get; set; }
        public string GdprUrl { get; set; }

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

        #endregion

        #region Urls

        [JsonProperty("thankYouPageUrl")]
        public string ThankYouPage { get; set; }

        #endregion

        [JsonProperty("allowedContentTypes")]
        public string[] AllowedContentTypes { get; set; }

        [JsonProperty("maxFileSize")]
        public int MaxAllFilesSize { get; set; }

        public OfferViewModel(DefinitionCombinationModel definition, ISettingsReaderService settingsReader) : base("Offer", settingsReader)
        {
            this.PageTitle = definition.OfferTitle.Text;
            this.MainText = definition.OfferMainText.Text;

            this.ThankYouPage = settingsReader.GetSiteSettings().ThankYou.Url;
        }
    }
}
