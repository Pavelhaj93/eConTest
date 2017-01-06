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

        public AuthenticationDataSessionStorage(Offer offer)
        {
            if ((offer == null) || (offer.OfferInternal.Body == null))
            {
                throw new OfferIsNullException("Offer is null by Session init");
            }

            var data = this.GetData();
            if ((data == null) || (data.Identifier != offer.OfferInternal.Body.Guid))
            {

                Random rnd = new Random();
                int value = rnd.Next(1, 4);

                AuthenticationDataItem authenticationDataItem = new AuthenticationDataItem();


                authenticationDataItem.DateOfBirth = offer.OfferInternal.Body.BIRTHDT;
                authenticationDataItem.Identifier = offer.OfferInternal.Body.Guid;
                authenticationDataItem.LastName = offer.OfferInternal.Body.NAME_LAST;
                authenticationDataItem.ExpDate = offer.OfferInternal.Body.DATE_TO;
                var generalSettings = ConfigHelpers.GetGeneralSettings();


                switch (value)
                {
                    case 1:
                        authenticationDataItem.ItemType = "PARTNER";
                        authenticationDataItem.ItemValue = offer.OfferInternal.Body.PARTNER;
                        authenticationDataItem.ItemFriendlyName = generalSettings.IdentityCardNumber;
                        break;

                    case 2:
                        authenticationDataItem.ItemType = "PSC_MS";
                        authenticationDataItem.ItemValue = offer.OfferInternal.Body.PscMistaSpotreby;
                        authenticationDataItem.ItemFriendlyName = generalSettings.UsedPostalCode;
                        break;

                    case 3:
                        authenticationDataItem.ItemType = "PSC_ADDR";
                        authenticationDataItem.ItemValue = offer.OfferInternal.Body.PscTrvaleBydliste;
                        authenticationDataItem.ItemFriendlyName = generalSettings.PermanentResidencePostalCode;
                        break;
                    default:
                        break;
                }
                HttpContext.Current.Session[SessionKey] = authenticationDataItem;
            }

        }

        public AuthenticationDataSessionStorage() { }

        public AuthenticationDataItem GetData()
        {
            if (HttpContext.Current.Session[SessionKey] != null)
            {
                var data = HttpContext.Current.Session[SessionKey] as AuthenticationDataItem;
                return data;
            }

            return null;
        }

        public Boolean IsDataActive()
        {
            return HttpContext.Current.Session[SessionKey] != null;
        }

        public static Boolean IsDataActiveStatic()
        {
            return HttpContext.Current.Session["AuthDataSession"] != null;
        }

        public void ClearSession()
        {
            HttpContext.Current.Session[SessionKey] = null;
        }
    }
}
