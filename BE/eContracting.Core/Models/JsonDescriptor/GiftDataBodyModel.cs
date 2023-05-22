using System.Collections.Generic;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class GiftDataBodyModel : IDataBodyModel
    {
        [JsonProperty("groups")]
        public IEnumerable<GiftDataGroupModel> Groups { get; set; }
    }
}
