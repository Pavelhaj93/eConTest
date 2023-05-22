using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class GiftDataHeaderModel : IDataHeaderModel
    {
        public string Title { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
