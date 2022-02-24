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

namespace eContracting.Rules.Conditions
{
    /// <summary>
    /// Evaluates if current logged-in user's offer has required process type.
    /// </summary>
    /// <typeparam name="T">The rule context.</typeparam>
    /// <seealso cref="Sitecore.Rules.Conditions.WhenCondition{T}" />
    public class WhenOfferHasSpecificProcessTypeCondition<T> : WhenCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the process type identifier.
        /// </summary>
        public string ProcessTypeId { get; set; }

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
        ///     <c>True</c> if match found in <see cref="UserCacheDataModel"/>, otherwise <c>false</c>.
        ///     Also <c>false</c> if <see cref="ProcessTypeId"/> doesn't exist or user is not logged in.
        /// </returns>
        protected override bool Execute(T ruleContext)
        {
            var context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
            var processType = context.GetItem<IProcessTypeModel>(this.ProcessTypeId);

            if (processType == null)
            {
                return false;
            }

            var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];
            var cache = ServiceLocator.ServiceProvider.GetRequiredService<IDataRequestCacheService>();
            var offer = cache.GetOffer(guid);

            if (offer == null)
            {
                return false;
            }

            return offer.ProcessType == processType.Code;
        }
    }
}
