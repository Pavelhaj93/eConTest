using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferProductModel
    {
        /// <summary>
        /// Gets or sets type of product.
        /// </summary>
        /// <value>e (electricity) | g (gas)</value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets basic information about user.
        /// </summary>
        /// <example>Smluvní údaje</example>
        [JsonProperty("header")]
        public HeaderData Header { get; set; }

        [JsonProperty("info_prices")]
        public IEnumerable<ProductInfoPrice> ProductInfos { get; set; }

        [JsonProperty("middle_texts")]
        public IEnumerable<string> MiddleTexts { get; set; }

        [JsonProperty("middle_texts_help", NullValueHandling = NullValueHandling.Ignore)]
        public string MiddleTextsHelp { get; set; }

        [JsonProperty("benefits")]
        public IEnumerable<string> Benefits { get; set; }

        public class HeaderData
        {
            /// <summary>
            /// Gets or sets product name
            /// </summary>
            /// <example>Optimal</example>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets price description 1.
            /// </summary>
            /// <example>Úspora</example>
            [JsonProperty("price1_description")]
            public string Price1Description { get; set; }

            /// <summary>
            /// Gets or sets price 1.
            /// </summary>
            /// <example>2420 Kč / rok</example>
            [JsonProperty("price1")]
            public string Price1Value { get; set; }

            [JsonProperty("price1_note")]
            public string Price1Note { get; set; }

            /// <summary>
            /// Gets or sest price description 2.
            /// </summary>
            /// <example>Roční odměna</example>
            [JsonProperty("price2_description")]
            public string Price2Description { get; set; }

            /// <summary>
            /// Gets or sets price 2.
            /// </summary>
            /// <example>500 Kč</example>
            [JsonProperty("price2")]
            public string Price2Value { get; set; }

            [JsonProperty("price2_note")]
            public string Price2Note { get; set; }
        }

        public class ProductInfoPrice
        {
            /// <summary>
            /// Gets or sets title for a product.
            /// </summary>
            /// <example>Odebraný plyn</example>
            [JsonProperty("title")]
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets previous strikethrough price.
            /// </summary>
            /// <example>800 Kč / MWh</example>
            [JsonProperty("previous_price")]
            public string PreviousPrice { get; set; }
            
            /// <summary>
            /// Gets or sets current price.
            /// </summary>
            /// <example>540</example>
            [JsonProperty("value")]
            public string Price { get; set; }

            /// <summary>
            /// Gets or sets current price value type.
            /// </summary>
            /// <example>Kč / MWh</example>
            [JsonProperty("value_unit")]
            public string PriceUnit { get; set; }
        }
    }
}
