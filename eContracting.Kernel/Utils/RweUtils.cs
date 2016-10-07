using System;
using System.Web;

namespace eContracting.Kernel.Utils
{
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
