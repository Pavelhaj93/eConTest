using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Authorization data model.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UserCacheDataModel
    {
        /// <summary>
        /// The authorization rokens.
        /// </summary>
        public OAuthTokensModel Tokens { get; set; }

        /// <summary>
        /// The Cognito user data.
        /// </summary>
        public CognitoUserModel CognitoUser { get; set; }

        /// <summary>
        /// Determinates is user has Cognito data.
        /// </summary>
        public bool IsCognito => this.CognitoUser != null;

        /// <summary>
        /// Gets pairs of guids and how they were authenticated.
        /// </summary>
        public IDictionary<string, AUTH_METHODS> AuthorizedGuids { get; } = new Dictionary<string, AUTH_METHODS>();

        public bool IsAnonymous => this.AuthorizedGuids.Count == 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserCacheDataModel"/> class.
        /// </summary>
        /// <remarks>Creates anonymous instance of the user.</remarks>
        public UserCacheDataModel()
        {
        }

        /// <summary>
        /// Determinates if user is authorized with <paramref name="authType"/> method.
        /// </summary>
        /// <param name="authType">The authentication method type.</param>
        /// <returns><c>True</c> is <see cref="AuthorizedGuids"/> contains value with <paramref name="authType"/>.</returns>
        public bool HasAuth(AUTH_METHODS authType)
        {
            return this.AuthorizedGuids.Values.Contains(authType);
        }

        /// <summary>
        /// Determinates if <paramref name="guid"/> is authorized with <see cref="AUTH_METHODS.COGNITO"/> or not.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        public bool IsCognitoGuid(string guid)
        {
            if (this.AuthorizedGuids.ContainsKey(guid))
            {
                if (this.AuthorizedGuids[guid] == AUTH_METHODS.COGNITO)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears authentication data.
        /// </summary>
        /// <remarks>
        /// Clears these data:
        /// <list type="bullet">
        /// <item><see cref="CognitoUser"/></item>
        /// <item><see cref="Tokens"/></item>
        /// <item><see cref="AuthorizedGuids"/></item>
        /// </list>
        /// </remarks>
        public void ClearAuthData()
        {
            this.CognitoUser = null;
            this.Tokens = null;
            this.AuthorizedGuids.Clear();
        }

        public void SetAuth(string guid, AUTH_METHODS authMethod)
        {
            this.AuthorizedGuids[guid] = authMethod;
        }

        public bool IsAuthFor(string guid)
        {
            return this.AuthorizedGuids.ContainsKey(guid);
        }

        public bool IsAuthFor(string guid, AUTH_METHODS authMethod)
        {
            return this.AuthorizedGuids.ContainsKey(guid) && this.AuthorizedGuids[guid] == authMethod;
        }

        public AUTH_METHODS GetAuthMethod(string guid)
        {
            if (!this.AuthorizedGuids.ContainsKey(guid))
            {
                return AUTH_METHODS.NONE;
            }

            return this.AuthorizedGuids[guid];
        }

        public string[] GetGuidsByAuthMethod(AUTH_METHODS authMethod)
        {
            return this.AuthorizedGuids.Where(x => x.Value == authMethod).Select(x => x.Key).ToArray();
        }

        public string[] GetAllAuthGuids()
        {
            return this.AuthorizedGuids.Select(x => x.Key).ToArray();
        }

        public bool RemoveAuth(string guid)
        {
            if (!this.AuthorizedGuids.ContainsKey(guid))
            {
                return false;
            }
            
            if (this.AuthorizedGuids[guid] == AUTH_METHODS.COGNITO)
            {
                this.CognitoUser = null;
                this.Tokens = null;
            }

            return this.AuthorizedGuids.Remove(guid);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"User: ");

            if (this.AuthorizedGuids.Count == 0)
            {
                builder.Append("anonymous.");
            }
            else
            {
                var guids = new List<string>();

                foreach (var item in this.AuthorizedGuids)
                {
                    guids.Add($"{item.Key} - {item.Value}");
                }

                builder.Append("Auths: ");
                builder.Append(string.Join(", ", guids));
            }

            builder.Append(" ");

            if (this.CognitoUser != null)
            {
                builder.Append($"{nameof(this.CognitoUser.PreferredUsername)}: {this.CognitoUser.PreferredUsername}");
                builder.Append(" ");
            }

            return builder.ToString();
        }
    }
}
