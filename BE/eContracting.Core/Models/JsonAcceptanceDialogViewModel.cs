using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonAcceptanceDialogViewModel
    {
        [JsonProperty("params")]
        public IEnumerable<JsonAcceptanceDialogParamViewModel> Parameters { get; set; }
    }
}
