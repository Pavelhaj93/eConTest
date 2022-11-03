using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferDistributorChangeModel
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <example>Níže je uveden obchodník, kterému innogy zašle výpověď</example>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets name of distributor.
        /// </summary>
        /// <example>Centropol</example>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <example>V případě, že jste podepsal smlouvu více obchoníkům (případně je obchodník neznámý), klikněte prosím ...</example>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
