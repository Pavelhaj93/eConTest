using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// The authentication provider.
        /// </summary>
        protected readonly IAuthenticationProviderService AuthenticationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="authProvider">The authentication provider.</param>
        /// <param name="settingsReader">The settings reader.</param>
        /// <exception cref="ArgumentNullException">
        /// authProvider
        /// or
        /// settingsReader
        /// </exception>
        public AuthenticationService(IAuthenticationProviderService authProvider, ISettingsReaderService settingsReader)
        {
            this.AuthenticationProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
        }

        /// <inheritdoc/>
        public string GetUniqueKey(LoginTypeModel loginType, OfferModel offer)
        {
            return Utils.GetMd5(loginType.ID.ToString() + offer.Guid);
        }

        /// <inheritdoc/>
        public AuthResultState GetLoginState(OfferModel offer, string birthDay, string key, string value)
        {
            if (string.IsNullOrEmpty(birthDay))
            {
                return AuthResultState.INVALID_BIRTHDATE;
            }

            var bd1 = offer.Birthday.Trim().Replace(" ", string.Empty).ToLower();
            var bd2 = birthDay.Trim().Replace(" ", string.Empty).ToLower();

            if (bd1 != bd2)
            {
                return AuthResultState.INVALID_BIRTHDATE;
            }

            var loginType = this.GetMatched(offer, key);

            if (loginType == null)
            {
                return AuthResultState.KEY_MISMATCH;
            }

            if (loginType.Name == "PARTNER")
            {
                if (offer.PartnerNumber != value)
                {
                    return AuthResultState.INVALID_PARTNER;
                }

                return AuthResultState.SUCCEEDED;
            }

            var val = value.Trim().Replace(" ", string.Empty).ToLower();

            if (loginType.Name == "PSC_ADDR")
            {
                if (offer.PostNumber != val)
                {
                    return AuthResultState.INVALID_ZIP1;
                }

                return AuthResultState.SUCCEEDED;
            }

            if (loginType.Name == "PSC_MS")
            {
                if (offer.PostNumberConsumption != val)
                {
                    return AuthResultState.INVALID_ZIP2;
                }

                return AuthResultState.SUCCEEDED;
            }

            return AuthResultState.KEY_VALUE_MISMATCH;
        }

        /// <inheritdoc/>
        public void Login(AuthDataModel authData)
        {
            this.AuthenticationProvider.Login(authData);
        }

        /// <inheritdoc/>
        public bool IsLoggedIn()
        {
            return this.AuthenticationProvider.IsLoggedIn();
        }

        /// <inheritdoc/>
        public AuthDataModel GetCurrentUser()
        {
            return this.AuthenticationProvider.GetData();
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
                if (key == this.GetUniqueKey(loginType, offer))
                {
                    return loginType;
                }
            }

            return null;
        }
    }
}
