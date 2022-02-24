using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public interface ITokenParser
    {
        JwtSecurityToken GetJwtToken(string accessToken);

        OfferTokenModel DecodeOfferToken(string token);

        string Encode(OfferTokenModel token);
    }
}
