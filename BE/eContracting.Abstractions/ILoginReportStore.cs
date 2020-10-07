using System;

namespace eContracting.Abstractions
{
    /// <summary>
    /// Stores information about login attemps.
    /// </summary>
    public interface ILoginReportStore
    {
        /// <summary>
        /// Adds the login attempt.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="birthdayDate">if set to <c>true</c> [birthday date].</param>
        /// <param name="wrongPostalCode">if set to <c>true</c> [wrong postal code].</param>
        /// <param name="WrongResidencePostalCode">if set to <c>true</c> [wrong residence postal code].</param>
        /// <param name="WrongIdentityCardNumber">if set to <c>true</c> [wrong identity card number].</param>
        /// <param name="generalError">if set to <c>true</c> [general error].</param>
        void AddLoginAttempt(string sessionId, string timestamp, string guid, string type, bool birthdayDate = false, bool wrongPostalCode = false, bool WrongResidencePostalCode = false, bool WrongIdentityCardNumber = false, bool generalError = false);

        /// <summary>
        /// Adds the failed login attempt.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="browserInfo">The browser information.</param>
        void AddFailedAttempt(string guid, string sessionId, string browserInfo);

        /// <summary>
        /// Determines whether <paramref name="guid"/> can log-in.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="maxFailedAttempts">The maximum failed attempts.</param>
        /// <param name="delayAfterFailedAttempts">The delay after failed attempts.</param>
        /// <returns>
        ///   <c>true</c> if this instance can login the specified unique identifier; otherwise, <c>false</c>.
        /// </returns>
        bool CanLogin(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts);
    }
}
