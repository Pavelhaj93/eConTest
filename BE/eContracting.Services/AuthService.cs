using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Services
{
    /// <summary>
    /// Not used
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AuthService : IAuthService
    {
        protected const string ISSUER = "eContracting";
        protected const string AUDIENCE = "eContracting";
        protected const string ORIGINAL_ISSUER = "innogy";

        public bool IsAuthenticated { get; }

        public JwtSecurityToken Authenticate(string guid, AUTH_METHODS authMethod)
        {
            if (authMethod == AUTH_METHODS.NONE)
            {
                return null;
            }

            var claims = new List<Claim>();
            claims.Add(new Claim("GUID", guid, ClaimValueTypes.String));
            claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Enum.GetName(typeof(AUTH_METHODS), authMethod)));
            var header = new JwtHeader();
            var payload = new JwtPayload(ISSUER, AUDIENCE, claims, DateTime.Now, DateTime.Now.AddHours(1));
            var token = new JwtSecurityToken(header, payload);
            return token;
        }

        public JwtSecurityToken GetJwtSecurityToken()
        {
            throw new NotImplementedException();
        }
    }
}
