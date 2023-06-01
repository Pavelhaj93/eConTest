using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class SummaryBenefitDataBodyModel : BenefitDataBodyModel
    {
        [JsonProperty("infos")]
        public IEnumerable<TitleAndValueModel> Infos { get; set; }        
    }
}
