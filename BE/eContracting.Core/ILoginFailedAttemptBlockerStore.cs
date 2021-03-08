using System;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Temporary storage for failed login attempts.
    /// </summary>
    public interface ILoginFailedAttemptBlockerStore
    {
        /// <summary>
        /// Adds the login attempt.
        /// </summary>
        /// <remarks>
        ///     Both <see cref="LoginFailureModel.IsBirthdateValid"/> and <see cref="LoginFailureModel.IsValueValid"/> cannot be true.
        ///     <para>Do not throw any exception!</para>
        /// </remarks>
        /// <param name="loginAttemptModel">Data about login attempt.</param>
        void Add(LoginFailureModel loginAttemptModel);

        /// <summary>
        /// Determines whether user with <paramref name="guid"/> is allowed to log-in.
        /// </summary>
        /// <remarks>
        ///     Do not throw any exception! Instead returns <c>true</c>.
        /// </remarks>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="maxFailedAttempts">The maximum failed attempts.</param>
        /// <param name="delayAfterFailedAttempts">The delay after failed attempts.</param>
        /// <returns>
        ///   <c>True</c> if <paramref name="guid"/> can log-in, otherwise <c>false</c>.
        /// </returns>
        bool IsAllowed(string guid, int maxFailedAttempts, TimeSpan delayAfterFailedAttempts);

        /// <summary>
        /// Removes failure login attempts for specific <see cref="OfferModel.Guid"/> and allows user to log-in.
        /// </summary>
        /// <remarks>Do not throw any exception!</remarks>
        /// <param name="guid">The offer unique identifier.</param>
        void Clear(string guid);

        /// <summary>
        /// Removes all records older than a specified date from the database.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="previewOnly">if true, just counts the records older than the date</param>
        /// <returns></returns>
        int DeleteAllOlderThan(DateTime date, bool previewOnly);

    }
}
