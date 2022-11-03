using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class CognitoSettingsModelTests
    {
        [Fact]
        public void When_Constructed_All_Properties_Are_Correctly_Set()
        {
            string cognitoBaseUrl = "https://cognito-idp.eu-west-1.amazonaws.com";
            string cognitoTokensUrl = "https://wednesday-hosteduiname.auth.eu-west-1.amazoncognito.com";
            string cognitoClientId = "432fgjg483202jgn29023";
            string cognitoCookiePrefix = "CognitoIdentityServiceProvider";
            string cognitoCookieUser = "LastAuthUser";
            string innogyLoginUrl = "https://test-prihlaseni.innogy.cz/signin.html";
            string innogyLogoutUrl = "https://test-prihlaseni.innogy.cz/sigout.html";
            string innogyRegistrationUrl = "https://test-prihlaseni.innogy.cz/signup.html";
            string innogyDashboardUrl = "https://test-innosvet.innogy.cz/dashboard";

            var model = new CognitoSettingsModel(cognitoBaseUrl, cognitoTokensUrl, cognitoClientId, cognitoCookiePrefix, cognitoCookieUser, innogyLoginUrl, innogyLogoutUrl, innogyRegistrationUrl, innogyDashboardUrl);

            Assert.Equal(cognitoBaseUrl, model.CognitoBaseUrl);
            Assert.Equal(cognitoTokensUrl, model.CognitoTokensUrl);
            Assert.Equal(cognitoClientId, model.CognitoClientId);
            Assert.Equal(cognitoCookiePrefix, model.CognitoCookiePrefix);
            Assert.Equal(cognitoCookieUser, model.CognitoCookieUser);
            Assert.Equal(innogyLoginUrl, model.InnogyLoginUrl);
            Assert.Equal(innogyLogoutUrl, model.InnogyLogoutUrl);
            Assert.Equal(innogyRegistrationUrl, model.InnogyRegistrationUrl);
            Assert.Equal(innogyDashboardUrl, model.InnogyDashboardUrl);
        }
    }
}
