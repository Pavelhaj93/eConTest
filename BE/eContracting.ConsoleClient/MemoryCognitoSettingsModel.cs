using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.ConsoleClient
{
    public class MemoryCognitoSettingsModel : CognitoSettingsModel
    {
        public MemoryCognitoSettingsModel() : base(
            "https://cognito-idp.eu-west-1.amazonaws.com",
            "https://ciam-tst-prihlaseni.auth.eu-west-1.amazoncognito.com",
            "1jgbghjpnh8cq4qa4838ehsq9l",
            "CognitoIdentityServiceProvider",
            "LastAuthUser", 
            "https://localhost/login",
            "https://localhost/logou",
            "https://localhost/reg",
            "https://localhost/dashboard")
        {
        }
    }
}
