using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class AcceptedOfferViewModel : BaseAppConfigViewModel
    {
        [JsonIgnore]
        public IPageAcceptedOfferModel Datasource { get; set; }

        [JsonIgnore]
        public string MainText { get; set; }

        [JsonProperty("offerUrl")]
        public readonly string OfferApi = "/api/econ/accepted";

        [JsonProperty("getFileUrl")]
        public readonly string FileApi = "/api/econ/file";

        [JsonProperty("abMatrixCombinationPixelUrl")]
        public string AbMatrixCombinationPixelUrl { get; set; }

        public AcceptedOfferViewModel(ISiteSettingsModel siteSettings) : base("AcceptedOffer", siteSettings)
        {
        }
    }
}
