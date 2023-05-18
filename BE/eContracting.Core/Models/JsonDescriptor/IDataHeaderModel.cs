using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public interface IDataHeaderModel
    {
        [JsonProperty("title")]
        string Title { get; set; }
    }
}
