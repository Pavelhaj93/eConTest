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

        protected readonly IOfferService OfferService;
        protected readonly ISitecoreService SitecoreService;

        public WhenOfferMatchesMatrixCondition() : this(
            ServiceLocator.ServiceProvider.GetRequiredService<IOfferService>(),
            ServiceLocator.ServiceProvider.GetRequiredService<ISitecoreService>())
        {
        }

        public WhenOfferMatchesMatrixCondition(IOfferService cacheService, ISitecoreService sitecoreService)
        {
            this.OfferService = cacheService;
            this.SitecoreService = sitecoreService;
        }

        protected override bool Execute(T ruleContext)
        {
            try
            {
                if (Guid.Empty == this.MatrixItemGuid)
                {
                    return false;
                }

                var guid = HttpContext.Current.Request.QueryString[Constants.QueryKeys.GUID];
                var user = this.OfferService.GetOffer(guid);

                if (user == null)
                {
                    return false;
                }

                var matrixItem = this.SitecoreService.GetItem<IDefinitionCombinationModel>(this.MatrixItemGuid, builder => builder.Language(ruleContext.Item.Language));
                
                if (matrixItem == null)
                {
                    return false;
                }

                return user.Process == matrixItem.Process?.Code && user.ProcessType == matrixItem.ProcessType?.Code;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Cannot resolve the rule", ex, this);
            }

            return false;
        }
    }
}
