using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using eContracting.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eContracting.Services
{
    /// <summary>
    /// AWS Cognito service.
    /// </summary>
    /// <seealso cref="https://docs.aws.amazon.com/cognito/latest/developerguide/token-endpoint.html"/>
    public class CognitoAuthService : ICognitoAuthService
    {
        protected const string ACCESS_TOKEN_COOKIE_NAME = "accessToken";
        protected const string ID_TOKEN_COOKIE_NAME = "idToken";
        protected const string REFRESH_TOKEN_COOKIE_NAME = "refreshToken";
        protected const string USER_DATA_COOKIE_NAME = "userData";

        protected readonly ISettingsReaderService SettingsReader;
        protected readonly ITokenParser TokenParser;
        protected readonly IRespApiService RestApiService;
        protected readonly ILogger Logger;

        private CognitoSettingsModel _settings;

        public CognitoAuthService(ISettingsReaderService settingsReader, ITokenParser tokenParser, IRespApiService restApiService, ILogger logger)
        {
            this.SettingsReader = settingsReader;
            this.TokenParser = tokenParser;
            this.RestApiService = restApiService;
            this.Logger = logger;
        }

        /// <inheritdoc/>
        public CognitoSettingsModel GetSettings()
        {
            if (this._settings != null)
            {
                return this._settings;
            }

            var settings = this.SettingsReader.GetCognitoSettings();

            if (string.IsNullOrWhiteSpace(settings.CognitoBaseUrl))
            {
                throw new EcontractingApplicationException(new ErrorModel("AUTH-COG-1", "Missing Cognito base URL"));
            }

            if (string.IsNullOrWhiteSpace(settings.CognitoClientId))
            {
                throw new EcontractingApplicationException(new ErrorModel("AUTH-COG-2", "Missing Cognito Client ID"));
            }

            if (string.IsNullOrWhiteSpace(settings.InnogyLoginUrl))
            {
                throw new EcontractingApplicationException(new ErrorModel("AUTH-COG-3", "Missing innogy login URL"));
            }

            if (string.IsNullOrWhiteSpace(settings.InnogyLogoutUrl))
            {
                throw new EcontractingApplicationException(new ErrorModel("AUTH-COG-4", "Missing innogy logout URL"));
            }

            this._settings = settings;
            return this._settings;
        }
        
        /// <inheritdoc/>
        public OAuthTokensModel GetTokens(HttpCookieCollection cookies)
        {
            if (cookies == null)
            {
                return null;
            }

            var settings = this.GetSettings();
            var cookieUser = this.GetCookieValue(cookies, this.GetCookieUserName(settings));

            if (string.IsNullOrEmpty(cookieUser))
            {
                return null;
            }

            var accessToken = this.GetCookieValue(cookies, this.GetCookieUserValue(settings, cookieUser, ACCESS_TOKEN_COOKIE_NAME));
            var idToken = this.GetCookieValue(cookies, this.GetCookieUserValue(settings, cookieUser, ID_TOKEN_COOKIE_NAME));
            var refreshToken = this.GetCookieValue(cookies, this.GetCookieUserValue(settings, cookieUser, REFRESH_TOKEN_COOKIE_NAME));

            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            if (string.IsNullOrEmpty(idToken))
            {
                return null;
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            return new OAuthTokensModel(accessToken, idToken, refreshToken, cookieUser);
        }

        /// <inheritdoc/>
        public CognitoUserModel GetUser(HttpCookieCollection cookies)
        {
            if (cookies == null)
            {
                return null;
            }

            var settings = this.GetSettings();
            var cookieUser = this.GetCookieValue(cookies, this.GetCookieUserName(settings));

            if (string.IsNullOrEmpty(cookieUser))
            {
                return null;
            }

            var userData = this.GetCookieValue(cookies, this.GetCookieUserValue(settings, cookieUser, USER_DATA_COOKIE_NAME));

            if (string.IsNullOrEmpty(userData))
            {
                return null;
            }

            var data = JsonConvert.DeserializeObject<CognitoUserDataModel>(HttpUtility.UrlDecode(userData));

            return new CognitoUserModel(data);
        }

        /// <inheritdoc/>
        public CognitoUserModel GetVerifiedUser(OAuthTokensModel tokens)
        {
            var settings = this.GetSettings();
            var result = this.GetVerifiedUser(settings, tokens);

            if (result == null)
            {
                return null;
            }

            return new CognitoUserModel(result);
        }

        /// <inheritdoc/>
        public OAuthTokensModel GetRefreshedTokens(OAuthTokensModel tokens)
        {
            var settings = this.GetSettings();
            var result = this.GetNewAccessToken(settings, tokens);

            if (result == null)
            {
                return null;
            }

            return new OAuthTokensModel(result.AccessToken, result.IdToken, result.RefreshToken, tokens.LastAuthUser);
        }

        protected string GetCookieValue(HttpCookieCollection cookies, string name)
        {
            if (!cookies.AllKeys.Contains(name))
            {
                return null;
            }

            return cookies[name]?.Value;
        }

        protected string GetCookieUserName(CognitoSettingsModel settings)
        {
            return $"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{settings.CognitoCookieUser}";
        }

        protected string GetCookieUserValue(CognitoSettingsModel settings, string username, string cookiePostfix)
        {
            var encodedUsername = HttpUtility.UrlEncode(username);
            return $"{settings.CognitoCookiePrefix}.{settings.CognitoClientId}.{encodedUsername}.{cookiePostfix}";
        }

        protected CognitoUserDataModel GetVerifiedUser(CognitoSettingsModel settings, OAuthTokensModel tokens)
        {
            var data = new JObject();
            data.Add("AccessToken", new JValue(tokens.AccessToken));

            using(var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/x-amz-json-1.1"))
            {
                using(var request = new HttpRequestMessage(HttpMethod.Post, new Uri(settings.CognitoBaseUrl)))
                {
                    request.Content = content;
                    request.Headers.Add("X-Amz-Target", "AWSCognitoIdentityProviderService.GetUser");
                    request.Headers.CacheControl = new CacheControlHeaderValue();
                    request.Headers.CacheControl.NoCache = true;
                    var response = this.RestApiService.GetResponse<CognitoUserDataModel>(request);
                    var result = response?.Data;
                    return result;
                }
            }
        }

        protected JwtRefreshTokenModel GetNewAccessToken(CognitoSettingsModel settings, OAuthTokensModel tokens)
        {
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("client_id", settings.CognitoClientId));
            data.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            data.Add(new KeyValuePair<string, string>("refresh_token", tokens.RefreshToken));
            data.Add(new KeyValuePair<string, string>("scope", "openid profile"));
            
            var content = new FormUrlEncodedContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.CognitoClientId + ":" + tokens.AccessToken));

            var url = new UriBuilder(settings.CognitoBaseUrl);
            url.Path = "/oauth2/token";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url.Uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicToken);
            request.Content = content;

            var response = this.RestApiService.GetResponse<JwtRefreshTokenModel>(request);
            return response?.Data;
        }

        public DateTime? GetTokenValidity(OAuthTokensModel tokens)
        {
            if (string.IsNullOrEmpty(tokens?.AccessToken))
            {
                return null;
            }

            var tokenModel = this.TokenParser.GetJwtToken(tokens.AccessToken);

            if (tokenModel == null)
            {
                return null;
            }

            return tokenModel.ValidTo;
        }
    }
}
