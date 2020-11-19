using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    /// <summary>
    /// Contains data from AD1 - <c>COMMODITY_OFFER_SUMMARY</c>.
    /// </summary>
    public class ComplexOfferPerexViewModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("params")]
        public ComplexOfferPerexParamViewModel[] Parameters { get; set; }
    }
}
