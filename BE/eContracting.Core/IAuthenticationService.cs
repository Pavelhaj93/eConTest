﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Represents collection of actions for authentication operation.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Logins the specified offer.
        /// </summary>
        /// <seealso cref="LoginTypeModel"/>
        /// <param name="offer">The offer.</param>
        /// <param name="loginType">The login type.</param>
        /// <param name="birthDay">The birth day.</param>
        /// <param name="key">The key of login type.</param>
        /// <param name="value">The value by login type.</param>
        /// <returns></returns>
        AUTH_RESULT_STATES GetLoginState(OfferModel offer, LoginTypeModel loginType, string birthDay, string key, string value);

        /// <summary>
        /// Log-in user with <paramref name="authData"/>.
        /// </summary>
        /// <param name="authData">Authentication data.</param>
        void Login(AuthDataModel authData);

        /// <summary>
        /// Determines whether user is logged in.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if is logged in; otherwise, <c>false</c>.
        /// </returns>
        bool IsLoggedIn();

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>Data or null.</returns>
        AuthDataModel GetCurrentUser();
    }
}