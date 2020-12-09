using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Tracks common user actions on a website.
    /// </summary>
    public interface IEventLogger
    {
        /// <summary>
        /// Adds information for new log event.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="message">The message.</param>
        void Add(string sessionId, string guid, EVENT_NAMES eventName, string message);

        /// <summary>
        /// Adds new event log.
        /// </summary>
        /// <param name="eventLog">The event log.</param>
        void Add(EventLogModel eventLog);
    }
}
