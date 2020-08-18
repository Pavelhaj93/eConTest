namespace eContracting.Kernel.Utils
{
    public class RweUtils
    {
        public AuthenticationDataSessionStorage authenticationDataSessionStorage { get; set; }

        public bool IsUserInSession()
        {
            if (authenticationDataSessionStorage == null)
            {
                authenticationDataSessionStorage = new AuthenticationDataSessionStorage();
            }

            if (authenticationDataSessionStorage.IsDataActive())
            {
                return true;
            }

            return false;
        }
    }
}
