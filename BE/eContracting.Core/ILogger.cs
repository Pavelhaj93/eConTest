using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Represents abstration layer for logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Add <c>info</c> log <paramref name="message"/>.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        void Info(string guid, string message);

        /// <summary>
        /// Add <c>debug</c> log <paramref name="message"/>.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        void Debug(string guid, string message);

        /// <summary>
        /// Add <c>warning</c> log <paramref name="message"/>.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        void Warn(string guid, string message);

        /// <summary>
        /// Add <c>error</c> log <paramref name="message"/>.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        void Error(string guid, string message);

        /// <summary>
        /// Add <c>error</c> log message from <paramref name="exception"/> with its details.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="exception">The exception.</param>
        void Error(string guid, Exception exception);

        /// <summary>
        /// Add <c>error</c> log <paramref name="message"/> with <paramref name="exception"/> details.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(string guid, string message, Exception exception);

        /// <summary>
        /// Add <c>fatal</c> log <paramref name="message"/>.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        void Fatal(string guid, string message);

        /// <summary>
        /// Add <c>fatal</c> log message from <paramref name="exception"/> with its details.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="exception">The exception.</param>
        void Fatal(string guid, Exception exception);

        /// <summary>
        /// Add <c>fatal</c> log <paramref name="message"/> with <paramref name="exception"/> details.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Fatal(string guid, string message, Exception exception);
    }
}
