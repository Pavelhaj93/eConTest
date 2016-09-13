using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace rweClient
{
    public class AuthenticationDataItem
    {
        public String ItemType { get; set; }
        public String ItemValue { get; set; }
    }

    public class AuthenticationDataSessionStorage
    {
        private readonly String SessionKey = "AuthDataSession";

        public AuthenticationDataSessionStorage(rweClient.SerializationClasses.Offer offer)
        {
            if ((offer == null) || (offer.Body == null))
            {
                throw new OfferIsNullException("Offer is null by Session init");
            }

            if (!this.IsDataActive())
            {
                Random rnd = new Random();
                int value = rnd.Next(1, 4);

                AuthenticationDataItem authenticationDataItem = new AuthenticationDataItem();

                switch (value)
                {
                    case 1:
                        authenticationDataItem.ItemType = "PARTNER";
                        authenticationDataItem.ItemValue = offer.Body.PARTNER;
                        break;

                    case 2:
                        authenticationDataItem.ItemType = "PSC_MS";
                        authenticationDataItem.ItemValue = offer.Body.PscMistaSpotreby;
                        break;

                    case 3:
                        authenticationDataItem.ItemType = "PSC_ADDR";
                        authenticationDataItem.ItemValue = offer.Body.PscTrvaleBydliste;
                        break;

                    case 4:
                        authenticationDataItem.ItemType = "ACCOUNT_NUMBER";
                        authenticationDataItem.ItemValue = offer.Body.ACCOUNT_NUMBER;
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

        public void ClearSession()
        {
            HttpContext.Current.Session[SessionKey] = null;
        }
    }

    public class RweUtils
    {
        public AuthenticationDataSessionStorage authenticationDataSessionStorage { get; set; }

        public static String RedirectSessionExpired { get; set; }

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
