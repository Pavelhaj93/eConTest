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
        
        [JsonIgnore]
        public readonly int Position;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFilesSectionModel"/> class.
        /// </summary>
        protected JsonFilesSectionModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFilesSectionModel"/> class.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="title">The title.</param>
        public JsonFilesSectionModel(IEnumerable<JsonFileModel> files, string title) : this(files, title, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFilesSectionModel"/> class.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="title">The title.</param>
        /// <param name="position">The position.</param>
        public JsonFilesSectionModel(IEnumerable<JsonFileModel> files, string title, int position)
        {
            this.Files = files;
            this.Title = title;
            this.Position = position;
        }
    }
}
