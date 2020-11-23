using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferAcceptedModel
    {
        [JsonProperty("groups")]
        public IEnumerable<JsonFilesSectionModel> Groups { get; set; }

        public JsonOfferAcceptedModel(IEnumerable<JsonFilesSectionModel> groups)
        {
            this.Groups = groups;
        }
    }
}
