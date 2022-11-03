using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Represents parser for tokens.
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Gets JWT instance from given <paramref name="accessToken"/>.
        /// </summary>
        /// <param name="accessToken">Access token for JWT.</param>
        /// <returns>Null or instance.</returns>
        JwtSecurityToken GetJwtToken(string accessToken);
    }
}
