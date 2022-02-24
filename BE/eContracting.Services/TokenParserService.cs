using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    public class TokenParserService : ITokenParser
    {
        public OfferTokenModel DecodeOfferToken(string token)
        {
            using (Aes.Create("OfferTokenModel"))
            {
                
            }
                throw new NotImplementedException();
        }

        public string Encode(OfferTokenModel token)
        {
            throw new NotImplementedException();
        }

        public JwtSecurityToken GetJwtToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(accessToken);
        }
    }
}
