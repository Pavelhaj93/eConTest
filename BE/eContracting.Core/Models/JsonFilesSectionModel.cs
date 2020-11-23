using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonFilesSectionModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("files")]
        public IEnumerable<JsonFileModel> Files { get; set; }

        public JsonFilesSectionModel()
        {
        }

        public JsonFilesSectionModel(IEnumerable<JsonFileModel> files, string title)
        {
            this.Files = files;
            this.Title = title;
        }
    }
}
