using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Mvc.Helpers;

namespace eContracting.Website
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static HtmlString eContractingPlaceholder(this SitecoreHelper helper, string placeholderName)
        {
            return helper.Placeholder(placeholderName);
        }

        public static eContractingHtmlHelper eContracting(this HtmlHelper htmlHelper)
        {
            return new eContractingHtmlHelper(htmlHelper);
        }
    }
}
