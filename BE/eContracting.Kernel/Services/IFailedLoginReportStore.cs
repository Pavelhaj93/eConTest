using System;

namespace eContracting.Kernel.Services
{
    /// <summary>
    /// Stores information about failed login attempts.
    /// </summary>
    public interface IFailedLoginReportStore
    {
        /// <summary>
        /// Adds the failed login attempt.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="browserInfo">The browser information.</param>
        //void AddFailedAttempt(string guid, string sessionId, string browserInfo);

        /// <summary>
        /// Determines whether <paramref name="guid"/> can login.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="maxFailedAttempts">The maximum failed attempts.</param>
        /// <param name="delayAfterFailedAttempts">The delay after failed attempts.</param>
        /// <returns>
        ///   <c>true</c> if this instance can login the specified unique identifier; otherwise, <c>false</c>.
        /// </returns>
        //bool CanLogin(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts);
    }
}
