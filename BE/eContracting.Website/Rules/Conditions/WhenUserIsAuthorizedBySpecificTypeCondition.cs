using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Glass.Mapper.Sc.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace eContracting.Website.Rules.Conditions
{
    /// <summary>
    /// Evaluates if current logged-in user's offer has required process.
    /// </summary>
    /// <typeparam name="T">The rule context.</typeparam>
    /// <seealso cref="Sitecore.Rules.Conditions.WhenCondition{T}" />
    public class WhenUserIsAuthorizedBySpecificTypeCondition<T> : WhenCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public string AuthTypeId
        {
            get
            {
                return this.AuthTypeGuid.ToString("N");
            }
            set
            {
                if (Guid.TryParse(value, out Guid guid))
                {
                    this.AuthTypeGuid = guid;
                }
                else
                {
                    throw new ArgumentException("Value is not valid GUID");
                }
            }
        }

        protected Guid AuthTypeGuid { get; set; }

        protected readonly IUserService UserService;
        protected readonly ISitecoreService SitecoreService;

        public WhenUserIsAuthorizedBySpecificTypeCondition() : this(
            ServiceLocator.ServiceProvider.GetRequiredService<IUserService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreService>())
        {
        }

        public WhenUserIsAuthorizedBySpecificTypeCondition(IUserService userService, ISitecoreService sitecoreService)
        {
            this.UserService = userService;
            this.SitecoreService = sitecoreService;
        }

        /// <summary>
        /// Executes the specified rule context.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        ///     <c>True</c> if match found in <see cref="UserCacheDataModel"/>, otherwise <c>false</c>.
        ///     Also <c>false</c> if <see cref="AuthTypeId"/> doesn't exist or user is not logged in.
        /// </returns>
        protected override bool Execute(T ruleContext)
        {
            try
            {
                if (Guid.Empty == this.AuthTypeGuid)
                {
                    return false;
                }

                if (!this.UserService.IsAuthorized())
                {
                    return false;
                }

                var authMethodItem = this.SitecoreService.GetItem<IBaseSitecoreModel>(this.AuthTypeGuid, builder => builder.Language(ruleContext.Item.Language));

                if (authMethodItem == null)
                {
                    return false;
                }

                if (Enum.TryParse(authMethodItem.Name, true, out AUTH_METHODS authMethod))
                {
                    var currentUser = this.UserService.GetUser();
                    return currentUser.HasAuth(authMethod);
                }

                return false;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot resolve the rule", ex, this);
            }

            return false;
        }
    }
}
