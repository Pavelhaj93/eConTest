using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ComplexOfferAcceptedViewModel
    {
        [JsonProperty("groups")]
        public IEnumerable<FilesSectionViewModel> Groups { get; set; }

        public ComplexOfferAcceptedViewModel(IEnumerable<FilesSectionViewModel> groups)
        {
            this.Groups = groups;
        }
    }
}
