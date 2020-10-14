using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;

namespace eContracting.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        protected readonly ISettingsReaderService SettingsReader;

        public AuthenticationService(ISettingsReaderService settingsReader)
        {
            this.SettingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetTypes(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerable<LoginTypeModel> GetTypes(string process, string processType)
        {
            throw new NotImplementedException();
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
