using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;

namespace eContracting.Services
{
    public interface IAuthenticationTypesService
    {
        /// <summary>
        /// Gets all available authentication method types.
        /// </summary>
        /// <returns>One or more types.</returns>
        IEnumerable<AuthenticationTypeModel> GetTypes(OfferModel offer);
    }
}
