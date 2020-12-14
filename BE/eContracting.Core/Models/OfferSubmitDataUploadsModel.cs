using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Represents upload group for submit an offer.
    /// </summary>
    public class OfferSubmitDataUploadsModel
    {
        /// <summary>
        /// Gets or sets the group key.
        /// </summary>
        [JsonProperty("group")]
        public string GroupKey { get; set; }

        /// <summary>
        /// Gets or sets the file keys.
        /// </summary>
        [JsonProperty("keys")]
        public IEnumerable<string> FileKeys { get; set; }
    }
}
