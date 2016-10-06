using System;
using System.Globalization;
using System.Web;
using Actum.RweClient.SerializationClasses;

namespace Actum.RweClient
{
    public class AuthenticationDataItem
    {
        public String ItemType { get; set; }
        public String ItemValue { get; set; }
        public String ItemFriendlyName { get; set; }
        public String DateOfBirth { get; set; }
        public String Identifier { get; set; }
        public String LastName { get; set; }
        public Boolean IsAccountNumber { get; set; }
        public String ExpDate { get; set; }
        public DateTime ExpDateConverted
        {
            get
            {
                DateTime outputDateTimeValue;
                DateTime.TryParseExact(this.ExpDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTimeValue);
                return outputDateTimeValue;
            }
        }
        public String ExpDateFormatted
        {
            get
            {
                return this.ExpDateConverted.ToString("dd.MM.yyy");
            }
        }
    }

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

    public class RweUtils
    {
        public AuthenticationDataSessionStorage authenticationDataSessionStorage { get; set; }

        public static String RedirectSessionExpired { get; set; }
        public static String RedirectUserHasBeenBlocked { get; set; }
        public static String AcceptedOfferRedirect { get; set; }
        public static String WrongUrlRedirect { get; set; }
        public static String OfferExpired { get; set; }

        public void IsUserInSession()
        {
            if (authenticationDataSessionStorage == null)
            {
                authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
            }

            if (authenticationDataSessionStorage.IsDataActive())
            {
                return;
            }

            HttpContext.Current.Response.Redirect(RedirectSessionExpired);
        }
    }
}
