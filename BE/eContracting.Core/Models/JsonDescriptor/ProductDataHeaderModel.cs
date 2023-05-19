using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class ProductDataHeaderModel : IDataHeaderModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public IEnumerable<ProductsDataDataModel> Data { get; set; }
    }    
}
