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
        public PageAcceptedOfferModel Datasource { get; set; }

        [JsonIgnore]
        public string MainText { get; set; }

        [JsonProperty("offerUrl")]
        public readonly string OfferApi = "/api/econ/accepted";

        [JsonProperty("getFileUrl")]
        public readonly string FileApi = "/api/econ/file";

        public AcceptedOfferViewModel(ISettingsReaderService settingsService) : base("AcceptedOffer", settingsService)
        {
        }
    }
}
