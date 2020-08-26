namespace eContracting.Kernel.Utils
{
    using System;
    using System.Web;
    using eContracting.Kernel.Exceptions;
    using eContracting.Kernel.Helpers;
    using eContracting.Kernel.Services;

    /// <summary>
    /// Implementaiton of storage for authentication session data.
    /// </summary>
    public class AuthenticationDataSessionStorage : IAuthenticationDataSessionStorage
    {
        /// <summary>
        /// Gets session key.
        /// </summary>
        private readonly String SessionKey = "AuthDataSession";
        protected readonly ISettingsReaderService SettingsReaderService;

        public AuthenticationDataSessionStorage(ISettingsReaderService settingsReaderService)
        {
            this.SettingsReaderService = settingsReaderService ?? throw new ArgumentNullException(nameof(settingsReaderService));
        }

        /// <summary>
        /// Get the user data.
        /// </summary>
        /// <param name="offer">Offer</param>
        /// <param name="generateRandom">Flag if random seed shoudl be set.</param>
        /// <returns>Returns <see cref="AuthenticationDataItem"/>.</returns>
        public AuthenticationDataItem GetUserData(Offer offer, bool generateRandom)
        {
            if ((offer == null) || (offer.OfferInternal.Body == null))
            {
                throw new OfferIsNullException("Offer is null by Session init");
            }


            AuthenticationDataItem authenticationDataItem = new AuthenticationDataItem()
            {
                DateOfBirth = offer.OfferInternal.Body.BIRTHDT,
                Identifier = offer.OfferInternal.Body.Guid,
                LastName = offer.OfferInternal.Body.NAME_LAST,
                ExpDate = offer.OfferInternal.Body.DATE_TO,
                IsAccepted = offer.OfferInternal.IsAccepted,
                IsRetention = offer.OfferInternal.Body.OfferIsRetention,
                IsAcquisition = offer.OfferInternal.Body.OfferIsAquisition,
                HasVoucher = offer.OfferInternal.Body.OfferHasVoucher,
                OfferIsExpired = offer.OfferInternal.State == "9" || offer.OfferInternal.Body.OfferIsExpired,
                Commodity = offer.OfferInternal.Body.EanOrAndEic,
                Campaign = offer.OfferInternal.Body.Campaign,
                CreatedAt = offer.OfferInternal.CreatedAt,
                IsIndi = string.IsNullOrEmpty(offer.OfferInternal.Body.Campaign)
            };

            if (generateRandom)
                SetRandomData(authenticationDataItem, offer);

            return authenticationDataItem;

        }

        /// <summary>
        /// Gets user data.
        /// </summary>
        /// <returns>Returns user data.</returns>
        public AuthenticationDataItem GetUserData()
        {
            var data = HttpContext.Current.Session[SessionKey] as AuthenticationDataItem;
            return data;
        }

        /// <summary>
        /// Gets if data are presented.
        /// </summary>
        public Boolean IsDataActive
        {
            get
            {
                return this.GetUserData() != null;
            }
        }

        /// <summary>
        /// Clears session data.
        /// </summary>
        public void ClearSession()
        {
            HttpContext.Current.Session[SessionKey] = null;
        }

        /// <summary>
        /// Stores data into the session.
        /// </summary>
        /// <param name="data">User authentication data.</param>
        public void Login(AuthenticationDataItem data)
        {
            HttpContext.Current.Session[SessionKey] = data;
        }

        /// <summary>
        /// Sets the random data.
        /// </summary>
        /// <param name="authenticationData">Authentication data.</param>
        /// <param name="offer">Offer</param>
        private void SetRandomData(AuthenticationDataItem authenticationData, Offer offer)
        {
            var generalSettings = this.SettingsReaderService.GetGeneralSettings();
            Random rnd = new Random();
            int value = rnd.Next(1, 4);

            switch (value)
            {
                case 1:
                    authenticationData.ItemValue = offer.OfferInternal.Body.PARTNER;
                    authenticationData.ItemFriendlyName = generalSettings.IdentityCardNumber;
                    break;

                case 2:
                    authenticationData.ItemValue = offer.OfferInternal.Body.PscMistaSpotreby;
                    authenticationData.ItemFriendlyName = generalSettings.UsedPostalCode;
                    break;

                case 3:
                    authenticationData.ItemValue = offer.OfferInternal.Body.PscTrvaleBydliste;
                    authenticationData.ItemFriendlyName = generalSettings.PermanentResidencePostalCode;
                    break;
                default:
                    break;
            }
        }
    }
}
