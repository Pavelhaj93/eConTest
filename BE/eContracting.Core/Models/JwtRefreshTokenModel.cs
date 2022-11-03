using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JwtRefreshTokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }

        [JsonProperty("id_token")]
        public string IdToken { get; private set; }

        [JsonProperty("token_type")]
        public string TokenType { get; private set; }

        [JsonProperty("expires_in")]
        public int ExpirationInSeconds { get; private set; }
    }
}
