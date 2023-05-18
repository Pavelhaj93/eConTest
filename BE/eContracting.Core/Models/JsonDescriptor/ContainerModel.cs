using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class ContainerModel
    {
        [JsonProperty("data")]
        public List<IDataModel> Data { get; } = new List<IDataModel>();
    }
}
