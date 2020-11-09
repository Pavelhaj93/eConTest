using System;
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
        protected readonly ICache Cache;

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
        public AuthenticationService(ISettingsReaderService settingsReader, ICache cacheService)
        {
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
            this.Cache = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <inheritdoc/>
        public AUTH_RESULT_STATES GetLoginState(OfferModel offer, string birthDay, string key, string value)
        {
            if (string.IsNullOrEmpty(birthDay))
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE;
            }

            var bd1 = offer.Birthday.Trim().Replace(" ", string.Empty).ToLower();
            var bd2 = birthDay.Trim().Replace(" ", string.Empty).ToLower();

            if (bd1 != bd2)
            {
                return AUTH_RESULT_STATES.INVALID_BIRTHDATE;
            }

            var loginType = this.GetMatched(offer, key);

            if (loginType == null)
            {
                return AUTH_RESULT_STATES.KEY_MISMATCH;
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

            #region How it was before
            //if (loginType.Key == "PARTNER")
            //{
            //    if (!this.IsRegexValid(loginType, value))
            //    {
            //        return AuthResultState.INVALID_PARTNER_FORMAT;
            //    }

            //    if (offer.PartnerNumber != value)
            //    {
            //        return AuthResultState.INVALID_PARTNER;
            //    }

            //    return AuthResultState.SUCCEEDED;
            //}

            //if (loginType.Key == "PSC_ADDR")
            //{
            //    if (!this.IsRegexValid(loginType, value))
            //    {
            //        return AuthResultState.INVALID_ZIP1_FORMAT;
            //    }

            //    var zip = offer.PostNumber.Trim().Replace(" ", string.Empty).ToLower();

            //    if (zip != val)
            //    {
            //        return AuthResultState.INVALID_ZIP1;
            //    }

            //    return AuthResultState.SUCCEEDED;
            //}

            //if (loginType.Key == "PSC_MS")
            //{
            //    if (!this.IsRegexValid(loginType, value))
            //    {
            //        return AuthResultState.INVALID_ZIP2_FORMAT;
            //    }

            //    var zip = offer.PostNumberConsumption.Trim().Replace(" ", string.Empty).ToLower();

            //    if (zip != val)
            //    {
            //        return AuthResultState.INVALID_ZIP2;
            //    }

            //    return AuthResultState.SUCCEEDED;
            //}

            //return AUTH_RESULT_STATES.KEY_VALUE_MISMATCH;
            #endregion
        }

        /// <inheritdoc/>
        public void Login(AuthDataModel authData)
        {
            this.Cache.AddToSession(Constants.CacheKeys.AUTH_DATA, authData);
        }

        /// <inheritdoc/>
        public bool IsLoggedIn()
        {
            return this.GetCurrentUser() != null;
        }

        /// <inheritdoc/>
        public AuthDataModel GetCurrentUser()
        {
            return this.Cache.GetFromSession<AuthDataModel>(Constants.CacheKeys.AUTH_DATA);
        }

        /// <summary>
        /// Find <see cref="LoginTypeModel"/> by <paramref name="offer"/> and <paramref name="key"/>.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <param name="key">The key.</param>
        /// <returns>Login type or null.</returns>
        protected LoginTypeModel GetMatched(OfferModel offer, string key)
        {
            var loginTypes = this.SettingsReader.GetAllLoginTypes();

            foreach (var loginType in loginTypes)
            {
                if (key == Utils.GetUniqueKey(loginType, offer))
                {
                    return loginType;
                }
            }

            return null;
        }

        protected bool IsRegexValid(LoginTypeModel loginType, string value)
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
