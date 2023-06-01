using System.Collections.Generic;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class BenefitDataBodyModel : IDataBodyModel
    {
        [JsonProperty("points")]
        public IEnumerable<ValueModel> Points { get; set; }        
    }
}
