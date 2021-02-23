using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WhenOfferHasSpecificProcessTypeCondition{T}"/> class.
        /// </summary>
        public WhenOfferHasSpecificProcessTypeCondition()
        {
        }

        /// <summary>
        /// Executes the specified rule context.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        ///     <c>True</c> if match found in <see cref="AuthDataModel"/>, otherwise <c>false</c>.
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

                var context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
                var processType = context.GetItem<ProcessTypeModel>(this.ProcessTypeItemGuid, ruleContext.Item.Language);

                if (processType == null)
                {
                    return false;
                }

                var cacheService = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
                var offerData = cacheService.Get<OfferCacheDataModel>(Constants.CacheKeys.OFFER_IDENTIFIER);

                if (offerData == null)
                {
                    return false;
                }

                return offerData.ProcessType == processType.Code;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot resolve the rule", ex, this);
            }

            return false;
        }
    }
}
