using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Represent login failed information base on <see cref="FailedLoginInfoModel.Guid"/>
    /// </summary>
    public class FailedLoginInfoModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the user unique identifier.
        /// </summary>
        [BsonElement("guid")]
        [BsonRequired]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets number of failed attempts.
        /// </summary>
        [BsonElement("failed_attempts")]
        public int FailedAttempts { get; set; }

        /// <summary>
        /// Gets or sets the last failed attempt timestamp.
        /// </summary>
        [BsonElement("last_failed_attempt_timestamp")]
        public DateTime LastFailedAttemptTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the failed attempts historical information.
        /// </summary>
        [BsonElement("failed_attempts_info")]
        public List<FailedLoginInfoDetailModel> FailedAttemptsInfo { get; set; } = new List<FailedLoginInfoDetailModel>();
    }
}
