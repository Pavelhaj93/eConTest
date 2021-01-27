using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace eContracting.Website.Rules.Conditions
{
    public class WhenOfferMatchesMatrixCondition<T> : WhenCondition<T> where T : RuleContext
    {
        public string MatrixId { get; set; }

        public WhenOfferMatchesMatrixCondition()
        {
        }

        protected override bool Execute(T ruleContext)
        {
            return false;
        }
    }
}
