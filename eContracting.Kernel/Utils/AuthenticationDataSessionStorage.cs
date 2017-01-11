using System;
using System.Web;
using eContracting.Kernel.Exceptions;
using eContracting.Kernel.Services;
using eContracting.Kernel.Helpers;

namespace eContracting.Kernel.Utils
{
    public class AuthenticationDataSessionStorage
    {
        private readonly String SessionKey = "AuthDataSession";


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
                OfferIsExpired = offer.OfferInternal.Body.OfferIsExpired,
            };

            if (generateRandom)
                SetRandomData(authenticationDataItem, offer);
            
            return authenticationDataItem;

        }


        public AuthenticationDataItem GetUserData()
        {
                var data = HttpContext.Current.Session[SessionKey] as AuthenticationDataItem;
                return data;
        }

        public Boolean IsDataActive
        {
            get
            {
                return this.GetUserData() != null;
            }
        }

        public void ClearSession()
        {
            HttpContext.Current.Session[SessionKey] = null;
        }
        public void Login(AuthenticationDataItem data)
        {
            HttpContext.Current.Session[SessionKey] = data;
        }

        private void SetRandomData(AuthenticationDataItem authenticationData, Offer offer)
        {
            var generalSettings = ConfigHelpers.GetGeneralSettings();
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
