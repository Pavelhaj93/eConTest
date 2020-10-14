using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;

namespace eContracting.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gets all available authentication method types.
        /// </summary>
        [Obsolete]
        IEnumerable<LoginTypeModel> GetTypes(OfferModel offer);

        /// <summary>
        /// Gets all available authentication method types.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        IEnumerable<LoginTypeModel> GetTypes(string process, string processType);

        /// <summary>
        /// Logins the specified offer.
        /// </summary>
        /// <seealso cref="LoginTypeModel"/>
        /// <param name="offer">The offer.</param>
        /// <param name="birthDay">The birth day.</param>
        /// <param name="key">The key of login type.</param>
        /// <param name="value">The value by login type.</param>
        /// <returns></returns>
        AuthResultState GetLoginState(OfferModel offer, string birthDay, string key, string value);

        /// <summary>
        /// Gets the key with combination of unique key of <paramref name="loginType"/> and unique key of <paramref name="offer"/>.
        /// </summary>
        /// <param name="loginType">Type of the login.</param>
        /// <param name="offer">The offer.</param>
        /// <returns></returns>
        string GetUniqueKey(LoginTypeModel loginType, OfferModel offer);
    }
}
