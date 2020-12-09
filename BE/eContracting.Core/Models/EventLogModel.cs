using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Represent a log event entry.
    /// </summary>
    public class EventLogModel
    {
        /// <summary>
        /// Time in UTC format when this event happened.
        /// </summary>
        public readonly DateTime Time;
        
        /// <summary>
        /// Session id.
        /// </summary>
        public readonly string SessionId;

        /// <summary>
        /// Offer guid identifier.
        /// </summary>
        public readonly string Guid;

        /// <summary>
        /// Type of the event.
        /// </summary>
        public readonly EVENT_NAMES Event;

        /// <summary>
        /// The additional message.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogModel"/> class.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="message">The message.</param>
        public EventLogModel(string sessionId, string guid, EVENT_NAMES eventName, string message)
            : this(DateTime.UtcNow, sessionId, guid, eventName, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogModel"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentException">
        /// Value cannot be empty - sessionId
        /// or
        /// Value cannot be empty - guid
        /// </exception>
        public EventLogModel(DateTime time, string sessionId, string guid, EVENT_NAMES eventName, string message)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException("Value cannot be empty", nameof(sessionId));
            }

            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentException("Value cannot be empty", nameof(guid));
            }

            this.Time = time;
            this.SessionId = sessionId;
            this.Guid = guid;
            this.Event = eventName;
            this.Message = message;
        }
    }
}
