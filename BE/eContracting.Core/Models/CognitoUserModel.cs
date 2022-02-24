using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    [Serializable]
    public class CognitoUserModel
    {
        /// <summary>
        /// ID of the user.
        /// </summary>
        public readonly string Sub;

        /// <summary>
        /// List of segments where use is signed in.
        /// </summary>
        public readonly IEnumerable<string> CustomSegments;

        /// <summary>
        /// Determinates if email was verified.
        /// </summary>
        public readonly bool EmailVerified;

        /// <summary>
        /// Real user ID.
        /// </summary>
        /// <remarks>
        /// Could be:
        /// <list type="bullet">
        ///     <item>ID from iWelcome for early registered user</item>
        ///     <item>ID from Cognito for newly registerd user</item>
        /// </list>
        /// </remarks>
        public readonly string PreferredUsername;

        /// <summary>
        /// Firstname of the user.
        /// </summary>
        public readonly string GivenName;

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public readonly string FamilyName;

        /// <summary>
        /// Email of the user.
        /// </summary>
        public readonly string Email;

        /// <summary>
        /// ID of the user. Should be the same as <see cref="Sub"/>.
        /// </summary>
        public readonly string Username;

        public CognitoUserModel(
            string sub,
            IEnumerable<string> customSegments,
            bool emailVerified,
            string preferredUsername,
            string givenName,
            string familyName,
            string email,
            string userName)
        {
            this.Sub = sub;
            this.CustomSegments = customSegments;
            this.EmailVerified = emailVerified;
            this.PreferredUsername = preferredUsername;
            this.GivenName = givenName;
            this.FamilyName = familyName;
            this.Email = email;
            this.Username = userName;
        }

        public CognitoUserModel(CognitoUserDataModel userData)
        {
            this.Sub = userData.UserAttributes.FirstOrDefault(x => x.Name == "sub")?.Value;
            this.CustomSegments = new List<string>() { userData.UserAttributes.FirstOrDefault(x => x.Name == "custom:segment")?.Value };
            this.EmailVerified = userData.UserAttributes.FirstOrDefault(x => x.Name == "email_verified")?.Value == "true";
            this.PreferredUsername = userData.UserAttributes.FirstOrDefault(x => x.Name == "preferred_username")?.Value;
            this.GivenName = userData.UserAttributes.FirstOrDefault(x => x.Name == "given_name")?.Value;
            this.FamilyName = userData.UserAttributes.FirstOrDefault(x => x.Name == "family_name")?.Value;
            this.Email = userData.UserAttributes.FirstOrDefault(x => x.Name == "email")?.Value;
            this.Username = userData.Username;
        }
    }
}
