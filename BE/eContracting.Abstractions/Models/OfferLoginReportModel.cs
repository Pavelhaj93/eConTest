using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eContracting.Abstractions.Models
{
    public class OfferLoginReportModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets session identifier.
        /// </summary>
        [BsonElement("session_id")]
        [BsonRequired]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets login time.
        /// </summary>
        [BsonElement("last_login_time")]
        [BsonRequired]
        public string LastLoginTime { get; set; }

        /// <summary>
        /// Gets or sets unique identifier.
        /// </summary>
        [BsonElement("guid")]
        [BsonRequired]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets offer type and identifier.
        /// </summary>
        [BsonElement("offer_type_identifier")]
        [BsonRequired]
        public string OfferTypeIdentifier { get; set; }

        /// <summary>
        /// Gets or sets number of wrong birthday dates.
        /// </summary>
        [BsonElement("wrong_birthday_date_count")]
        public int WrongBirthdayDateCount { get; set; }

        /// <summary>
        /// Gets or sets number of wrong postal codes.
        /// </summary>
        [BsonElement("wrong_postal_code_count")]
        public int WrongPostalCodeCount { get; set; }

        /// <summary>
        /// Gets or sets number of wrong residence postal codes.
        /// </summary>
        [BsonElement("wrong_residence_postal_code_count")]
        public int WrongResidencePostalCodeCount { get; set; }

        /// <summary>
        /// Gets or sets number of wrong identity cards.
        /// </summary>
        [BsonElement("wrong_identity_card_number_count")]
        public int WrongIdentityCardNumberCount { get; set; }

        /// <summary>
        /// Gets or sets number general errors.
        /// </summary>
        [BsonElement("general_error_count")]
        public int GeneralErrorCount { get; set; }

        /// <summary>
        /// Gets or sets number of success attenpts.
        /// </summary>
        [BsonElement("success_attempt_count")]
        public int SuccessAttemptCount { get; set; }
    }
}
