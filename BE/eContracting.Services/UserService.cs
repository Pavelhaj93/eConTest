using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.UI.HtmlControls;
using eContracting.Models;
using static Glass.Mapper.Sc.Global;

namespace eContracting.Services
{
    public class UserService : IUserService
    {
        protected readonly ICognitoAuthService CognitoAuthService;
        protected readonly ISettingsReaderService SettingsReader;
        protected readonly IDataSessionCacheService Cache;
        protected readonly IContextWrapper ContextWrapper;
        protected readonly ILogger Logger;

        public UserService(
            ICognitoAuthService cognitoAuthService,
            ISettingsReaderService settingsReader,
            IDataSessionCacheService cacheService,
            IContextWrapper contextWrapper,
            ILogger logger)
        {
            this.CognitoAuthService = cognitoAuthService ?? throw new ArgumentNullException(nameof(cognitoAuthService));
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
            this.Cache = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            this.ContextWrapper = contextWrapper ?? throw new ArgumentNullException(nameof(contextWrapper));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public void SaveUser(string guid, UserCacheDataModel user)
        {
            this.Cache.Set(Constants.CacheKeys.USER_DATA, user);
            this.Logger.Debug(guid, "User data saved");
        }

        /// <inheritdoc/>
        /// <remarks>Only checks cookies for Cognito user.</remarks>
        public bool CanAuthenticate(string guid)
        {
            var cookies = this.ContextWrapper.GetCookies();

            if (cookies != null)
            {
                var tokens = this.CognitoAuthService.GetTokens(cookies);

                return tokens != null;
            }

            return true;
        }

        /// <inheritdoc/>
        public void Authenticate(string guid, UserCacheDataModel user)
        {
            //if (user.AuthorizedGuids.Count == 0)
            //{
            //    throw new EcontractingApplicationException(new ErrorModel(null, "Cannot authentication user without valid authentication method defined in AuthType property"));
            //}

            this.SaveUser(guid, user);

            var log = new StringBuilder();
            log.AppendLine("Successfully log-ged in via " + Enum.GetName(typeof(AUTH_METHODS), user.GetAuthMethod(guid)));
            log.AppendLine(" - Browser agent: " + this.ContextWrapper.GetBrowserAgent());
            this.Logger.Info(guid, log.ToString());
        }

        public bool TryAuthenticateUser(string guid, UserCacheDataModel user)
        {
            var cookies = this.ContextWrapper.GetCookies();
            var tokens = this.CognitoAuthService.GetTokens(cookies);

            if (tokens == null)
            {
                return false;
            }

            if (user.IsCognito)
            {
                if (user.Tokens.AccessToken != tokens.AccessToken)
                {
                    this.Logger.Info(guid, "Request tokens don't match to user's tokens");
                    this.Logout(guid, user);
                    return false;
                }
            }

            var verifiedUser = this.CognitoAuthService.GetVerifiedUser(tokens);

            if (verifiedUser == null)
            {
                this.Logger.Info(guid, "Cognito tokens are not valid - cannot verify user");
                this.Logout(guid, user);
                return false;
            }

            user.Tokens = tokens;
            user.CognitoUser = verifiedUser;
            user.SetAuth(guid, AUTH_METHODS.COGNITO);
            this.Authenticate(guid, user);
            this.Logger.Debug(guid, $"Cognito data added to '{user}'");
            return true;
        }

        /// <inheritdoc/>
        public bool TryUpdateUserFromContext(string guid, UserCacheDataModel user)
        {
            if (user.IsCognito)
            {
                this.Logger.Debug(guid, $"'{user}' already has congnito data");
                return true;
            }

            var cookies = this.ContextWrapper.GetCookies();
            var tokens = this.CognitoAuthService.GetTokens(cookies);

            if (tokens == null)
            {
                this.Logger.Debug(guid, "Cognito tokens not found in the request");
                return false;
            }

            var verifiedUser = this.CognitoAuthService.GetVerifiedUser(tokens);

            if (verifiedUser == null)
            {
                this.Logger.Info(guid, "Cognito tokens are not valid - cannot verify user");
                return false;
            }

            user.Tokens = tokens;
            user.CognitoUser = verifiedUser;
            //this.Authenticate(guid, user);
            this.Logger.Debug(guid, $"Cognito data added to '{user}'");
            this.RefreshAuthorizationIfNeeded(guid, user);
            return true;
        }

        /// <inheritdoc/>
        public void Logout(string guid)
        {
            var user = this.GetUser();

            if (user == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(guid))
            {
                this.Abandon(null);
                this.Logger.Info(null, "User is completely logout due to missing guid");
                return;
            }

            if (!user.IsAuthFor(guid))
            {
                this.Logger.Debug(guid, "User auth data not changed (guid not found in collection)");
                return;
            }

            var authMethod = user.GetAuthMethod(guid);

            if (authMethod != AUTH_METHODS.NONE)
            {
                this.Logout(guid, user, authMethod);
            }
        }

        public void Logout(string guid, UserCacheDataModel user)
        {
            user.ClearAuthData();
            this.SaveUser(guid, user);
        }

        /// <inheritdoc/>
        public void Logout(string guid, UserCacheDataModel user, AUTH_METHODS authMethod)
        {
            if (authMethod == AUTH_METHODS.NONE)
            {
                throw new EcontractingApplicationException(new ErrorModel("", $"Cannot logout user with authentication method '{AUTH_METHODS.NONE}'"));
            }

            if (authMethod == AUTH_METHODS.TWO_SECRETS)
            {
                user.RemoveAuth(guid);
                //var guids = user.AuthorizedGuids.Where(x => x.Value == AUTH_METHODS.TWO_SECRETS).Select(x => x.Key).ToArray();

                //foreach (var g in guids)
                //{
                //    user.AuthorizedGuids.RemoveAuth(g);
                //}
            }
            else
            {
                var guids = user.GetGuidsByAuthMethod(authMethod);

                foreach (var g in guids)
                {
                    user.RemoveAuth(g);
                }

                user.Tokens = null;
                user.CognitoUser = null;
            }

            this.SaveUser(guid, user);
            this.Logger.Debug(guid, $"'{user}' auth data for {authMethod} removed.");
        }

        /// <inheritdoc/>
        public void Abandon(string guid)
        {
            var user = this.GetUser();
            user = null;
            this.Cache.Remove(Constants.CacheKeys.USER_DATA);

            if (!string.IsNullOrEmpty(guid))
            {
                this.Logger.Debug(guid, "User was removed / abandoned");
            }
        }

        /// <inheritdoc/>
        public UserCacheDataModel GetUser()
        {
            var user = this.Cache.Get<UserCacheDataModel>(Constants.CacheKeys.USER_DATA);

            if (user != null)
            {
                return user;
            }

            this.Logger.Debug(null, "User doesn't exist, creating new one ...");
            var newUser = new UserCacheDataModel();
            this.SaveUser(null, newUser);
            return newUser;
        }

        /// <inheritdoc/>
        public bool IsAuthorized()
        {
            var user = this.GetUser();

            if (user.IsCognito)
            {
                return true;
            }

            if (user.GetAllAuthGuids().Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool IsAuthorizedFor(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return false;
            }

            var user = this.GetUser();

            //if (!user.Guids.Contains(guid))
            if (!user.IsAuthFor(guid))
            {
                this.Logger.Debug(guid, $"User is not authorized for current guid");
                return false;
            }

            return this.IsAuthorized(user, guid);
        }

        /// <inheritdoc/>
        public bool IsAuthorized(UserCacheDataModel user, string guid)
        {
            this.Logger.Debug(guid, $"Checking if '{user}' is authorized");

            if (user == null)
            {
                return false;
            }

            if (user.GetAllAuthGuids().Length == 0 && !user.IsCognito)
            {
                return false;
            }

            if (user.HasAuth(AUTH_METHODS.COGNITO))
            {
                return this.IsUserValid(guid, user);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool IsUserValid(string guid, UserCacheDataModel user)
        {
            if (user.IsCognito)
            {
                var cookies = this.ContextWrapper.GetCookies();

                if (cookies != null)
                {
                    var tokens = this.CognitoAuthService.GetTokens(cookies);

                    if (tokens == null)
                    {
                        this.Logger.Info(guid, $"Cannot get cookies for '{user}'");
                        return false;
                    }

                    if (!user.CognitoUser.Email.Equals(tokens.LastAuthUser, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.Logger.Info(guid, $"Current cookies don't match to current '{user}' (emails not match)");
                        return false;
                    }
                }
                else
                {
                    this.Logger.Debug(guid, "Cannot check tokens from cookies -> cookies not available");
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public void RefreshAuthorizationIfNeeded(string guid, UserCacheDataModel user)
        {
            if (user == null || !user.IsCognito || user.Tokens == null)
            {
                return;
            }

            var refreshTokens = this.GetRefreshTokens(user.Tokens);

            if (refreshTokens != null)
            {
                user.Tokens = refreshTokens;
                this.Logger.Info(guid, $"Access token for '{user}' has been refreshed");
            }
        }

        /// <summary>
        /// Gets new refresh token only when validity of current <paramref name="tokens"/> is less than <see cref="ISettingsReaderService.CognitoMinSecondsToRefreshToken"/>.
        /// </summary>
        /// <param name="tokens">The actual tokens.</param>
        protected internal OAuthTokensModel GetRefreshTokens(OAuthTokensModel tokens)
        {
            var validTo = this.CognitoAuthService.GetTokenValidity(tokens);

            if (!validTo.HasValue)
            {
                return null;
            }

            var seconds = (validTo.Value - DateTime.UtcNow).TotalSeconds;

            if (seconds > this.SettingsReader.CognitoMinSecondsToRefreshToken)
            {
                return null;
            }

            var refreshedTokens = this.CognitoAuthService.GetRefreshedTokens(tokens);

            if (refreshedTokens == null || string.IsNullOrEmpty(refreshedTokens.AccessToken))
            {
                return null;
            }

            return refreshedTokens;
        }
    }
}
