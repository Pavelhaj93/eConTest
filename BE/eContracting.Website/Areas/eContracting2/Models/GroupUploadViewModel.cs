using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class GroupUploadViewModel
    {
        /// <summary>
        /// Gets or sets group unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public readonly string Id;

        /// <summary>
        /// Gets or sets overall group size after optimization.
        /// </summary>
        [JsonProperty("size")]
        public readonly int Size;

        /// <summary>
        /// Gets or sets files successfully paired with the group.
        /// </summary>
        [JsonProperty("files")]
        public readonly IEnumerable<GroupUploadFileViewModel> Files;

        public GroupUploadViewModel(DbUploadGroupFileModel model)
        {
            this.Id = model.Id.ToString();
            this.Size = model.OutputFile.Size;
            this.Files = model.OriginalFiles.Select(x => new GroupUploadFileViewModel(x));
        }
    }
}
