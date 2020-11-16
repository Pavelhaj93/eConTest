using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.ConsoleClient.Commands
{
    class FileModel
    {
        [JsonProperty("IDATTACH")]
        public string IdAttach { get; set; }
        [JsonProperty("PRINTED")]
        public string Printed { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }

        public FileModel(string idAttach, string printed, string label)
        {
            this.IdAttach = idAttach;
            this.Printed = printed;
            this.Label = label;
        }
    }
}
