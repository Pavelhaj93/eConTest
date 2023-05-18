using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class BenefitDataBodyModel : IDataBodyModel
    {
        [JsonProperty("infos")]
        public IEnumerable<TitleAndValueModel> Infos { get; set; }

        [JsonProperty("points")]
        public IEnumerable<ValueModel> Points { get; set; }        
    }
}
