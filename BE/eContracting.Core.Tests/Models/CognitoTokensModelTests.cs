using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class CognitoTokensModelTests
    {
        [Fact]
        public void When_Constructed_All_Properties_Are_Correctly_Set()
        {
            string accessToken = "gj8s9f7gja-3iu4";
            string idToken = "jg8g8jsck9g720";
            string refreshToken = "hj89c0923h79v-2";
            string lastAuthUser = "lukas.dvorak@actumdigital.com";

            var model = new OAuthTokensModel(accessToken, idToken, refreshToken, lastAuthUser);

            Assert.Equal(accessToken, model.AccessToken);
            Assert.Equal(idToken, model.IdToken);
            Assert.Equal(refreshToken, model.RefreshToken);
            Assert.Equal(lastAuthUser, model.LastAuthUser);
        }
    }
}
