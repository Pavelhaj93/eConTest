using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Represent set of accepted, signed and uploaded documents for an offer.
    /// </summary>
    public class OfferSubmitDataModel
    {
        /// <summary>
        /// Gets or sets array of <c>accepted</c> unique file IDs.
        /// </summary>
        [JsonProperty("accepted")]
        public IEnumerable<string> Accepted { get; set; }

        /// <summary>
        /// Gets or sets array of <c>signed</c> unique file IDs.
        /// </summary>
        [JsonProperty("signed")]
        public IEnumerable<string> Signed { get; set; }

        /// <summary>
        /// Gets or sets array of <c>uploaded</c> unique file IDs.
        /// </summary>
        [JsonProperty("uploaded")]
        public IEnumerable<OfferSubmitDataUploadsModel> Uploaded { get; set; }

        /// <summary>
        /// Gets or sets array of <c>other</c> unique file IDs.
        /// </summary>
        [JsonProperty("other")]
        public IEnumerable<string> Other { get; set; }

        public string[] GetCheckedFiles()
        {
            var list = new List<string>();

            if (this.Accepted?.Count() > 0)
            {
                list.AddRange(this.Accepted);
            }

            if (this.Signed?.Count() > 0)
            {
                list.AddRange(this.Signed);
            }

            if (this.Other?.Count() > 0)
            {
                list.AddRange(this.Other);
            }

            return list.ToArray();
        }
    }
}
