using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class CancelOfferViewModel
    {
        [JsonProperty("cancelOfferUrl")]
        public string ButtonConfirmLink { get; } = "/api/econ/cancel";

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }
    }
}
