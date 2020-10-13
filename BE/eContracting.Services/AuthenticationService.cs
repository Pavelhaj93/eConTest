using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services.Models;

namespace eContracting.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public IEnumerable<LoginTypeModel> GetTypes(OfferModel offer)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LoginTypeModel> GetTypes(string process, string processType)
        {
            throw new NotImplementedException();
        }

        public AuthResultModel Login(OfferModel offer, string birthDay, string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
