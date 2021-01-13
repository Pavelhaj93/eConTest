using eContracting.Kernel.Services;

namespace eContracting.Kernel.Utils
{
    public interface IAuthenticationDataSessionStorage
    {
        bool IsDataActive { get; }

        void ClearSession();
        AuthenticationDataItem GetUserData();
        AuthenticationDataItem GetUserData(Offer offer, bool generateRandom);
        void Login(AuthenticationDataItem data);
    }
}
