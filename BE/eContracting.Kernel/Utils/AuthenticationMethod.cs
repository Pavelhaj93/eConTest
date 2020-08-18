using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Kernel.Models;
using eContracting.Kernel.Services;

namespace eContracting.Kernel.Utils
{
    public abstract class AuthenticationMethod
    {
        protected readonly AuthenticationDataSessionStorage sessionStorage;

        protected AuthenticationMethod(AuthenticationDataSessionStorage authSessionStorage)
        {
            this.sessionStorage = authSessionStorage;
        }

        public abstract AuthenticationDataItem GetUserData();

        public abstract IEnumerable<AuthenticationSettingsItemModel> GetAvailableAuthenticationFields();

        public abstract string GetRealAdditionalValue(string key);

    }
}
