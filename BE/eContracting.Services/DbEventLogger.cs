using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Storage;

namespace eContracting.Services
{
    /// <summary>
    /// Database storage for log events.
    /// </summary>
    /// <seealso cref="eContracting.IEventLogger" />
    [ExcludeFromCodeCoverage]
    public class DbEventLogger : IEventLogger
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbEventLogger"/> class.
        /// </summary>
        /// <param name="settingsReader">The settings reader.</param>
        /// <param name="logger">The logger.</param>
        public DbEventLogger(ISettingsReaderService settingsReader, ILogger logger)
        {
            this.ConnectionString = settingsReader.GetCustomDatabaseConnectionString();
            this.Logger = logger;
        }

        /// <inheritdoc/>
        public void Add(string sessionId, string guid, EVENT_NAMES eventName)
        {
            this.Add(sessionId, guid, eventName, null);
        }

        /// <inheritdoc/>
        public void Add(string sessionId, string guid, EVENT_NAMES eventName, string message)
        {
            var model = new EventLogModel(sessionId, guid, eventName, message);
            this.Add(model);
        }

        /// <inheritdoc/>
        public void Add(EventLogModel eventLog)
        {
            try
            {
                using (var context = new DatabaseContext(this.ConnectionString))
                {
                    var record = new EventLog(eventLog);
                    context.EventLogs.Add(record);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(eventLog.Guid, "Cannot save new log event data", ex);
            }
        }
    }
}
