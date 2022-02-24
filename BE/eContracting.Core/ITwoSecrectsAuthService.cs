using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Represents collection of actions for authentication operation.
    /// </summary>
    public interface ITwoSecrectsAuthService
    {
        /// <summary>
        /// Log-in user with <paramref name="authData"/>.
        /// </summary>
        /// <param name="authData">Authentication data.</param>
        void Authenticate(UserCacheDataModel authData);

        /// <summary>
        /// Determines whether user is logged in via cookies or not.
        /// </summary>
        bool IsAuthenticated(HttpRequestBase request);

        /// <summary>
        /// Determines whether user is logged in via cookies or not.
        /// </summary>
        bool IsAuthenticated(HttpRequestContext request);

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>Data or null.</returns>
        UserCacheDataModel GetCurrentUser();
    }
}
