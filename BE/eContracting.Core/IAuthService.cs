using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    public interface IAuthService
    {
        bool IsAuthenticated { get; }

        JwtSecurityToken GetJwtSecurityToken();

        JwtSecurityToken Authenticate(string guid, AUTH_METHODS authMethod);
    }
}
