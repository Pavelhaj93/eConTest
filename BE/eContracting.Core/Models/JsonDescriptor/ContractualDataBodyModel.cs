using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class ContractualDataBodyModel : IDataBodyModel
    {
        [JsonProperty("personalData")]
        public IEnumerable<TitleAndValuesModel> PersonalData { get; set; }

        [JsonProperty("addresses")]
        public IEnumerable<TitleAndValuesModel> Addresses { get; set; }

        [JsonProperty("contacts")]
        public IEnumerable<TitleAndValuesModel> Contacts { get; set; }
    }
}
