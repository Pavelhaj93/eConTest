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
    /// Evaluates if current logged-in user's offer has required process.
    /// </summary>
    /// <typeparam name="T">The rule context.</typeparam>
    /// <seealso cref="Sitecore.Rules.Conditions.WhenCondition{T}" />
    [Obsolete]
    public class WhenOfferHasSpecificProcessCondition<T> : WhenCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public string ProcessId { get; set; }

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
            var context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
            var process = context.GetItem<IProcessModel>(this.ProcessId);

            if (process == null)
            {
                return false;
            }

            var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];
            var service = ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>();
            
            var offer = service.GetOffer(guid);

            if (offer == null)
            {
                return false;
            }

            return offer.Process == process.Code;
        }
    }
}
