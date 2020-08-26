namespace eContracting.Kernel.Services
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
        void AddOfferLoginAttempt(string sessionId, string timestamp, string guid, string type, bool birthdayDate = false, bool wrongPostalCode = false, bool WrongResidencePostalCode = false, bool WrongIdentityCardNumber = false, bool generalError = false);
    }
}
