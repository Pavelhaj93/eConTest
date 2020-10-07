using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace eContracting.Abstractions.Models
{
    /// <summary>
    /// Represents detail about failed login
    /// </summary>
    public class FailedLoginInfoDetailModel
    {
        /// <summary>
        /// Gets or sets when it happend.
        /// </summary>
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets session ID.
        /// </summary>
        [BsonElement("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the browser information.
        /// </summary>
        [BsonElement("browser_info")]
        public string BrowserInfo { get; set; }
    }
}
