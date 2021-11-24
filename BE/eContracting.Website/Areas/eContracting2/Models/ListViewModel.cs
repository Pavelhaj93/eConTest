using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ListViewModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("items")]
        public IEnumerable<ListItemViewModel> Items { get; set; }

        public ListViewModel()
        {
        }

        public ListViewModel(IListCollectionModel model)
        {
            this.Items = model.Items.Select(x => new ListItemViewModel(x));
        }
    }
}
