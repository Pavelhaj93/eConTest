﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;

namespace eContracting.Services
{
    /// <inheritdoc/>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// The settings reader.
        /// </summary>
        protected readonly ISettingsReaderService SettingsReader;

        /// <summary>
        /// The cache.
        /// </summary>
        protected readonly IUserDataCacheService Cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="settingsReader">The settings reader.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <exception cref="ArgumentNullException">
        /// settingsReader
        /// or
        /// cacheService
        /// </exception>
        public AuthenticationService(ISettingsReaderService settingsReader, IUserDataCacheService cacheService)
        {
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
            this.Cache = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <inheritdoc/>
        public AUTH_RESULT_STATES GetLoginState(OfferModel offer, LoginTypeModel loginType, string birthDay, string key, string value)
        {
            if (string.IsNullOrEmpty(birthDay))
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE;
            }

            if (string.IsNullOrEmpty(key))
            {
                return AUTH_RESULT_STATES.KEY_MISMATCH;
            }

            if (string.IsNullOrEmpty(value))
            {
                return AUTH_RESULT_STATES.MISSING_VALUE;
            }

            var bd1 = offer.Birthday.Trim().Replace(" ", string.Empty).ToLower();
            var bd2 = birthDay.Trim().Replace(" ", string.Empty).ToLower();

            if (bd1 != bd2)
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE;
            }

            if (loginType == null)
            {
                return AUTH_RESULT_STATES.MISSING_LOGIN_TYPE;
            }

            var val = value.Trim().Replace(" ", string.Empty); //.Replace(" ", string.Empty).ToLower();

            // We can do it this way but it strongly depends on editor what he defines as 'loginType.Name'.
            var xmlValue = offer.GetValue(loginType.Key)?.Trim().Replace(" ", string.Empty);

            if (string.IsNullOrEmpty(xmlValue))
            {
                return AUTH_RESULT_STATES.INVALID_VALUE_DEFINITION;
            }

            if (!this.IsRegexValid(loginType, val))
            {
                return AUTH_RESULT_STATES.INVALID_VALUE_FORMAT;
            }

            if (xmlValue != val)
            {
                return AUTH_RESULT_STATES.INVALID_VALUE;
            }

            return AUTH_RESULT_STATES.SUCCEEDED;
        }

        /// <inheritdoc/>
        public void Login(AuthDataModel authData)
        {
            this.Cache.Set(Constants.CacheKeys.AUTH_DATA, authData);
        }

        /// <inheritdoc/>
        public bool IsLoggedIn()
        {
            return this.GetCurrentUser() != null;
        }

        /// <inheritdoc/>
        public AuthDataModel GetCurrentUser()
        {
            return this.Cache.Get<AuthDataModel>(Constants.CacheKeys.AUTH_DATA);
        }

        protected internal bool IsRegexValid(LoginTypeModel loginType, string value)
        {
            if (string.IsNullOrEmpty(loginType.ValidationRegex))
            {
                return true;
            }

            try
            {
                return Regex.IsMatch(value, loginType.ValidationRegex);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}