using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Represents page model for error page 404.
    /// </summary>
    /// <seealso cref="eContracting.Models.BaseSitecoreModel" />
    [SitecoreType(TemplateId = "{6CCFEF10-7A13-47F8-AAF3-237085B9C556}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class Error404PageModel : BaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        [SitecoreField]
        public virtual string PageTitle { get; set; }

        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        [SitecoreField]
        public virtual string Text { get; set; }
    }
}
