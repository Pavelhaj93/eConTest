using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    /// <summary>
    /// Represents JSON model from userData cookie.
    /// </summary>
    public class CognitoUserDataModel
    {
        [JsonProperty("UserAttributes")]
        public UserAttribute[] UserAttributes { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        public class UserAttribute
        {
            [JsonProperty("Name")]
            public string Name { get; set; }

            [JsonProperty("Value")]
            public string Value { get; set; }
        }
    }
}
