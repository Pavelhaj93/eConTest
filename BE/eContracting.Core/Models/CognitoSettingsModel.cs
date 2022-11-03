using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class CognitoSettingsModel
    {
        /// <summary>
        /// Base URL for Cognito service.
        /// </summary>
        public readonly string CognitoBaseUrl;

        public readonly string CognitoTokensUrl;

        /// <summary>
        /// OAuth Client ID for Cognito service.
        /// </summary>
        public readonly string CognitoClientId;

        public readonly string CognitoCookiePrefix;

        public readonly string CognitoCookieUser;
        
        /// <summary>
        /// Base URL for login page on innogy.cz.
        /// </summary>
        public readonly string InnogyLoginUrl;

        /// <summary>
        /// Base URL for logout page on innogy.cz.
        /// </summary>
        public readonly string InnogyLogoutUrl;

        public readonly string InnogyRegistrationUrl;

        public readonly string InnogyDashboardUrl;

        public CognitoSettingsModel(string cognitoBaseUrl, string cognitoTokensUrl, string cognitoClientId, string cognitoCookiePrefix, string cognitoCookieUser, string innogyLoginUrl, string innogyLogoutUrl, string innogyRegistrationUrl, string innogyDashboardUrl)
        {
            this.CognitoBaseUrl = cognitoBaseUrl;
            this.CognitoTokensUrl = cognitoTokensUrl;
            this.CognitoClientId = cognitoClientId;
            this.CognitoCookiePrefix = cognitoCookiePrefix;
            this.CognitoCookieUser = cognitoCookieUser;
            this.InnogyLoginUrl = innogyLoginUrl;
            this.InnogyLogoutUrl = innogyLogoutUrl;
            this.InnogyRegistrationUrl = innogyRegistrationUrl;
            this.InnogyDashboardUrl = innogyDashboardUrl;
        }
    }
}
