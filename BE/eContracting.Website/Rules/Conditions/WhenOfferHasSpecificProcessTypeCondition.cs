using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using eContracting.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace eContracting.Website.Rules.Conditions
{
    /// <summary>
    /// Evaluates if current logged-in user's offer has required process type.
    /// </summary>
    /// <typeparam name="T">The rule context.</typeparam>
    /// <seealso cref="Sitecore.Rules.Conditions.WhenCondition{T}" />
    public class WhenOfferHasSpecificProcessTypeCondition<T> : WhenCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public string ProcessTypeId
        {
            get
            {
                return this.ProcessTypeItemGuid.ToString("N");
            }
            set
            {
                if (Guid.TryParse(value, out Guid guid))
                {
                    this.ProcessTypeItemGuid = guid;
                }
                else
                {
                    throw new ArgumentException("Value is not valid GUID");
                }
            }
        }

        protected Guid ProcessTypeItemGuid { get; set; }

        protected readonly IRequestDataCacheService CacheService;
        protected readonly ISitecoreService SitecoreService;
        protected readonly IOfferService OfferService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WhenOfferHasSpecificProcessTypeCondition{T}"/> class.
        /// </summary>
        public WhenOfferHasSpecificProcessTypeCondition() : this(
            ServiceLocator.ServiceProvider.GetRequiredService<IRequestDataCacheService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>())
        {
        }

        public WhenOfferHasSpecificProcessTypeCondition(
            IRequestDataCacheService cacheService,
            ISitecoreService sitecoreService,
            IOfferService offerService)
        {
            this.CacheService = cacheService;
            this.SitecoreService = sitecoreService;
            this.OfferService = offerService;
        }

        /// <summary>
        /// Executes the specified rule context.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        ///     <c>True</c> if match found in <see cref="UserCacheDataModel"/>, otherwise <c>false</c>.
        ///     Also <c>false</c> if <see cref="ProcessTypeId"/> doesn't exist or user is not logged in.
        /// </returns>
        protected override bool Execute(T ruleContext)
        {
            try
            {
                if (Guid.Empty == this.ProcessTypeItemGuid)
                {
                    return false;
                }

                var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];
                var user = this.OfferService.GetOffer(guid);

                if (user == null)
                {
                    return false;
                }

                var processType = this.SitecoreService.GetItem<IProcessTypeModel>(this.ProcessTypeItemGuid, builder => builder.Language(ruleContext.Item.Language));

                if (processType == null)
                {
                    return false;
                }

                return user.ProcessType == processType.Code;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot resolve the rule", ex, this);
            }

            return false;
        }
    }
}
