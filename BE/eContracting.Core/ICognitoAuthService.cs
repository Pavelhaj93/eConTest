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
    /// Cognito authorization service.
    /// </summary>
    /// <seealso cref="https://aws.amazon.com/blogs/mobile/understanding-amazon-cognito-user-pool-oauth-2-0-grants/"/>
    public interface ICognitoAuthService
    {
        /// <summary>
        /// Gets settings.
        /// </summary>
        /// <returns></returns>
        CognitoSettingsModel GetSettings();

        /// <summary>
        /// Gets model with available tokens.
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns>Model or null if cookies are not available.</returns>
        OAuthTokensModel GetTokens(HttpCookieCollection cookies);

        /// <summary>
        /// Gets user data Cognito model.
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns>Model or null if cookie are not available.</returns>
        CognitoUserModel GetUser(HttpCookieCollection cookies);

        /// <summary>
        /// Gets user data from Cognito remote service.
        /// </summary>
        /// <param name="tokens">Cognito authorization tokens.</param>
        CognitoUserModel GetVerifiedUser(OAuthTokensModel tokens);

        DateTime? GetTokenValidity(OAuthTokensModel tokens);

        OAuthTokensModel GetRefreshedTokens(OAuthTokensModel tokens);
    }
}
