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
        [JsonProperty("size")]
        public readonly IEnumerable<GroupUploadFileViewModel> Files;

        public GroupUploadViewModel(OptimizedFileGroupModel model)
        {
            this.Id = model.Id;
            this.Size = model.Size;
            this.Files = model.Files.Select(x => new GroupUploadFileViewModel(x));
        }
    }
}
