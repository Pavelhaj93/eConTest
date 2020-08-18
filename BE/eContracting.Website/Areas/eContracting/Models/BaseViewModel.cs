using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting.Models
{
    public class BaseViewModel
    {
        [JsonProperty("view")]
        public string View { get; set; }

        public BaseViewModel(string view)
        {
            this.View = view;
        }
    }
}
