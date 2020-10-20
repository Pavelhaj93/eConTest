using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public abstract class BaseViewModel
    {
        public string PageTitle { get;set; }

        [JsonProperty("view")]
        public string View { get; set; }

        public BaseViewModel(string view)
        {
            this.View = view;
        }
    }
}
