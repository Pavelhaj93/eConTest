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
    /// Evaluates if current logged-in user has specific authentication method assigned.
    /// </summary>
    /// <typeparam name="T">The rule context.</typeparam>
    /// <seealso cref="Sitecore.Rules.Conditions.WhenCondition{T}" />
    public class WhenOfferHasSpecificProcessCondition<T> : WhenCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public string ProcessId
        {
            get
            {
                return this.ProcessItemGuid.ToString("N");
            }
            set
            {
                if (Guid.TryParse(value, out Guid guid))
                {
                    this.ProcessItemGuid = guid;
                }
                else
                {
                    throw new ArgumentException("Value is not valid GUID");
                }
            }
        }

        protected Guid ProcessItemGuid { get; set; }

        protected readonly IDataRequestCacheService CacheService;
        protected readonly ISitecoreService SitecoreService;

        public WhenOfferHasSpecificProcessCondition() : this(
            ServiceLocator.ServiceProvider.GetRequiredService<IDataRequestCacheService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreService>())
        {
        }

        public WhenOfferHasSpecificProcessCondition(IDataRequestCacheService cacheService, ISitecoreService sitecoreService)
        {
            this.CacheService = cacheService;
            this.SitecoreService = sitecoreService;
        }

        /// <summary>
        /// Executes the specified rule context.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        ///     <c>True</c> if match found in <see cref="UserCacheDataModel"/>, otherwise <c>false</c>.
        ///     Also <c>false</c> if <see cref="ProcessId"/> doesn't exist or user is not logged in.
        /// </returns>
        protected override bool Execute(T ruleContext)
        {
            try
            {
                if (Guid.Empty == this.ProcessItemGuid)
                {
                    return false;
                }

                var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];
                var user = this.CacheService.GetOffer(guid);

                if (user == null)
                {
                    return false;
                }

                var process = this.SitecoreService.GetItem<IProcessModel>(this.ProcessItemGuid, builder => builder.Language(ruleContext.Item.Language));

                if (process == null)
                {
                    return false;
                }

                return user.Process == process.Code;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot resolve the rule", ex, this);
            }

            return false;
        }
    }
}
