using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ListItemViewModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public ListItemViewModel()
        {
        }

        public ListItemViewModel(ListItemModel model)
        {
            this.Label = model.Text;
            this.Value = model.Value;
        }
    }
}
