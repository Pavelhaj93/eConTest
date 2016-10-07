using System;
using System.Globalization;
using System.Web;
using eContracting.Kernel.Exceptions;
using eContracting.Kernel.Services;

namespace eContracting.Kernel.Utils
{
    public class AuthenticationDataSessionStorage
    {
        private readonly String SessionKey = "AuthDataSession";

        public AuthenticationDataSessionStorage(Offer offer)
        {
            if ((offer == null) || (offer.Body == null))
            {
                throw new OfferIsNullException("Offer is null by Session init");
            }

            var data = this.GetData();
            if ((data == null) || (data.Identifier != offer.Body.Guid))
            {

                Random rnd = new Random();
                int value = rnd.Next(1, 4);

                AuthenticationDataItem authenticationDataItem = new AuthenticationDataItem();

                DateTime outputDateTimeValue;
                if (DateTime.TryParseExact(offer.Body.BIRTHDT, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTimeValue))
                {
                    authenticationDataItem.DateOfBirth = outputDateTimeValue.ToString("dd.MM.yyy");
                }
                else
                {
                    throw new DateOfBirthWrongFormatException(String.Format("Wrong format: {0}", offer.Body.BIRTHDT));
                }

                authenticationDataItem.Identifier = offer.Body.Guid;
                authenticationDataItem.LastName = offer.Body.NAME_LAST;
                authenticationDataItem.ExpDate = offer.Body.DATE_TO;

                switch (value)
                {
                    case 1:
                        authenticationDataItem.ItemType = "PARTNER";
                        authenticationDataItem.ItemValue = offer.Body.PARTNER;
                        authenticationDataItem.ItemFriendlyName = "Číslo OP";
                        authenticationDataItem.IsAccountNumber = false;
                        break;

                    case 2:
                        authenticationDataItem.ItemType = "PSC_MS";
                        authenticationDataItem.ItemValue = offer.Body.PscMistaSpotreby;
                        authenticationDataItem.ItemFriendlyName = "PSČ Místa spotřeby";
                        authenticationDataItem.IsAccountNumber = false;
                        break;

                    case 3:
                        authenticationDataItem.ItemType = "PSC_ADDR";
                        authenticationDataItem.ItemValue = offer.Body.PscTrvaleBydliste;
                        authenticationDataItem.ItemFriendlyName = "PSČ trvalého bydliště";
                        authenticationDataItem.IsAccountNumber = false;
                        break;

                    case 4:
                        authenticationDataItem.ItemType = "ACCOUNT_NUMBER";
                        authenticationDataItem.ItemValue = offer.Body.ACCOUNT_NUMBER;
                        authenticationDataItem.ItemFriendlyName = "číslo bankovního účtu včetně kódu banky za lomítkem";
                        authenticationDataItem.IsAccountNumber = true;
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
