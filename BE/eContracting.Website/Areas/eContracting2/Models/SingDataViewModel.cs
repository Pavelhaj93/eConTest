using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class SingDataViewModel
    {
        /// <summary>
        /// Gets or sets posted signature from sign dialog in format for inline image.
        /// </summary>
        /// <value>
        /// The signature in format like 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAv4AAACMCAYAAAAEEK1aAAAaS0lEQVR4Xu3de/B1VV3....'.
        /// </value>
        [JsonProperty("signature")]
        [DataMember(Name = "signature")]
        public string Signature { get; set; }
    }
}
