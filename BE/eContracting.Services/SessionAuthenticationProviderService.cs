using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Services.Models;

namespace eContracting.Services
{
    /// <summary>
    /// Session provider for logged user.
    /// </summary>
    /// <seealso cref="eContracting.Services.IAuthenticationProviderService" />
    public class SessionAuthenticationProviderService : IAuthenticationProviderService
    {
        /// <inheritdoc/>
        public AuthDataModel GetData()
        {
            return HttpContext.Current.Session[Constants.SessionKeys.NAME] as AuthDataModel;
        }

        /// <inheritdoc/>
        public bool IsLoggedIn()
        {
            return this.GetData() != null;
        }

        /// <inheritdoc/>
        public void Login(AuthDataModel authData)
        {
            HttpContext.Current.Session[Constants.SessionKeys.NAME] = authData;
        }
    }
}
