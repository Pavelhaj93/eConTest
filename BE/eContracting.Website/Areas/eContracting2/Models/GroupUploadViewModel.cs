using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class GroupUploadViewModel
    {
        /// <summary>
        /// Gets or sets group unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets overall group size after optimization.
        /// </summary>
        [JsonProperty("size")]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets files successfully paired with the group.
        /// </summary>
        [JsonProperty("size")]
        public IEnumerable<GroupUploadFileViewModel> Files { get; set; }
    }
}
