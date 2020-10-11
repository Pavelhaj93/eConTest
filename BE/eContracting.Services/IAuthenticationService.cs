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
        IEnumerable<AuthenticationTypeModel> GetTypes(OfferModel offer);

        /// <summary>
        /// Gets all available authentication method types.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="processType">Type of the process.</param>
        IEnumerable<AuthenticationTypeModel> GetTypes(string process, string processType);
    }
}
