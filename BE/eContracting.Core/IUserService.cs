using System;
using System.Web;
using System.Web.Http.Controllers;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Represents user storage and authentication / authorization service.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Saves data about current (not authorized user). This is used for first load to store data about offer.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">The user data.</param>
        void SaveUser(string guid, UserCacheDataModel user);

        /// <summary>
        /// Determinates is user could be authenticated from current context data.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        bool CanAuthenticate(string guid);

        /// <summary>
        /// Authenticate <paramref name="user"/> and store data.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user"></param>
        void Authenticate(string guid, UserCacheDataModel user);

        bool TryAuthenticateUser(string guid, UserCacheDataModel user);

        /// <summary>
        /// Try to update <paramref name="user"/> auth data from current request.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user"></param>
        [Obsolete("User instead 'TryAuthenticateUser'")]
        bool TryUpdateUserFromContext(string guid, UserCacheDataModel user);

        /// <summary>
        /// Removes auth data from current user.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        void Logout(string guid);

        void Logout(string guid, UserCacheDataModel user);

        /// <summary>
        /// Removes auth data from <paramref name="user"/> for specific <paramref name="authMethod"/>.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="authMethod">Authentication method.</param>
        void Logout(string guid, UserCacheDataModel user, AUTH_METHODS authMethod);

        /// <summary>
        /// Removes current user from cache.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        void Abandon(string guid);

        /// <summary>
        /// Determinates if user is authenticated for current request.
        /// </summary>
        bool IsAuthorized();

        /// <summary>
        /// Determinates is current user is authorized for given <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        bool IsAuthorizedFor(string guid);

        /// <summary>
        /// Determinates if <paramref name="user"/> is authorized or not.
        /// </summary>
        /// <param name="user">Cache model with user data.</param>
        /// <param name="guid">The unique offer identifier.</param>
        bool IsAuthorized(UserCacheDataModel user, string guid);

        /// <summary>
        /// Checks if current user is valid in current context.
        /// </summary>
        /// <param name="guid">The unique offer identifier.</param>
        /// <param name="user">Cache model with user data.</param>
        bool IsUserValid(string guid, UserCacheDataModel user);

        /// <summary>
        /// Gets the current user data or new anonymous.
        /// </summary>
        /// <returns>Always returns the instance.</returns>
        UserCacheDataModel GetUser();

        /// <summary>
        /// Determinates if a user is stored or not.
        /// </summary>
        bool UserExists();

        /// <summary>
        /// Refreshes authorization data if it's necessary.
        /// </summary>
        /// <param name="user">Cache model with user data.</param>
        /// <param name="guid">The unique offer identifier.</param>
        void RefreshAuthorizationIfNeeded(string guid, UserCacheDataModel user);
    }
}
