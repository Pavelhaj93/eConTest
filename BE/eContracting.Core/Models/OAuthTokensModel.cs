using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    [Serializable]
    public class OAuthTokensModel
    {
        /// <summary>
        /// A valid user pool access token.
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// A valid user pool ID token. Note that an ID token is only provided if the openid scope was requested.
        /// </summary>
        public string IdToken { get; private set; }

        /// <summary>
        /// A valid user pool refresh token.
        /// </summary>
        /// <remarks>
        ///  This can be used to retrieve new tokens by sending it through a POST request to https://AUTH_DOMAIN/oauth2/token, specifying the refresh_token and client_id parameters, and setting the grant_type parameter to “refresh_token“.
        /// </remarks>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// User identifier.
        /// </summary>
        public string LastAuthUser { get; private set; }

        protected OAuthTokensModel()
        {
        }

        public OAuthTokensModel(string accessToken, string idToken, string refreshToken, string lastAuthUser)
        {
            this.AccessToken = accessToken;
            this.IdToken = idToken;
            this.RefreshToken = refreshToken;
            this.LastAuthUser = lastAuthUser;
        }

        public void UpdateTokens(OAuthTokensModel tokens)
        {
            this.AccessToken = tokens.AccessToken;
        }
    }
}
