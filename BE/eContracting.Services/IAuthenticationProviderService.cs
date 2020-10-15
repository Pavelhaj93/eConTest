using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Services.Models;

namespace eContracting.Services
{
    /// <summary>
    /// Represents storage provider which holds information about logged users.
    /// </summary>
    public interface IAuthenticationProviderService
    {
        /// <summary>
        /// Log-in user with <paramref name="authData"/> containing <see cref="AuthDataModel.Guid"/>.
        /// </summary>
        /// <param name="authData">Auth data model.</param>
        void Login(AuthDataModel authData);

        /// <summary>
        /// Determines whether user is logged in.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if is logged in; otherwise, <c>false</c>.
        /// </returns>
        bool IsLoggedIn();

        /// <summary>
        /// Gets auth date for current (context) user.
        /// </summary>
        /// <returns>Data or null.</returns>
        AuthDataModel GetData();
    }
}
