using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;
using Glass.Mapper.Sc;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace eContracting.Website.Rules.Conditions
{
    public class WhenOfferMatchesMatrixCondition<T> : WhenCondition<T> where T : RuleContext
    {
        public string MatrixId
        {
            get
            {
                return this.MatrixItemGuid.ToString("N");
            }
            set
            {
                if (Guid.TryParse(value, out Guid guid))
                {
                    this.MatrixItemGuid = guid;
                }
                else
                {
                    throw new ArgumentException("Value is not valid GUID");
                }
            }
        }

        protected Guid MatrixItemGuid { get; set; }

        public WhenOfferMatchesMatrixCondition()
        {
        }

        protected override bool Execute(T ruleContext)
        {
            try
            {
                if (Guid.Empty == this.MatrixItemGuid)
                {
                    return false;
                }

                var cacheService = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
                var offerData = cacheService.Get<OfferCacheDataModel>(Constants.CacheKeys.OFFER_IDENTIFIER);

                if (offerData == null)
                {
                    return false;
                }

                var context = ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreContext>();
                var matrixItem = context.GetItem<IDefinitionCombinationModel>(this.MatrixItemGuid, ruleContext.Item.Language);
                
                if (matrixItem == null)
                {
                    return false;
                }

                return offerData.Process == matrixItem.Process?.Code && offerData.ProcessType == matrixItem.ProcessType?.Code;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot resolve the rule", ex, this);
            }

            return false;
        }
    }
}
