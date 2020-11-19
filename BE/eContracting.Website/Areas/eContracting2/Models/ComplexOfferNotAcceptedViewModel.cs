using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ComplexOfferNotAcceptedViewModel
    {
        [JsonProperty("perex")]
        public ComplexOfferPerexViewModel Perex { get; set; }

        [JsonProperty("gifts")]
        public ComplexOfferGiftsViewModel Gifts { get; set; }
    }
}
