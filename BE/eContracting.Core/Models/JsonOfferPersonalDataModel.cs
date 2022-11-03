using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonOfferPersonalDataModel
    {
        /// <summary>
        /// Gets or sets title for personal info.
        /// </summary>
        /// <example>Smluvní údaje</example>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets personal data collection.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<PersonalDataInfo> Infos { get; set; }

        /// <summary>
        /// Gets or sets agreed method of commnication.
        /// </summary>
        [JsonProperty("communication", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CommunicationMethodInfo AgreedCommunicationMethod { get; set; }

        public class PersonalDataInfo
        {
            /// <summary>
            /// Gets or sets title of personal info.
            /// </summary>
            /// <example>Osobní údaje | Adresa trvalého bydliště | Korespondenční adresa</example>
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("lines")]
            public IEnumerable<string> Lines { get; set; }
        }

        public class CommunicationMethodInfo
        {
            /// <summary>
            /// Gets or sets title for agreed method of commnication.
            /// </summary>
            /// <example>Sjednaný způsob komunikace</example>
            public string Title { get; set; }

            /// <summary>
            /// Gets or set type of agreed method of communication.
            /// </summary>
            /// <example>innosvět</example>
            public string Value { get; set; }
        }
    }
}
