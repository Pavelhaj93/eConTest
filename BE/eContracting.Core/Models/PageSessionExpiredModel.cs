using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{1740614A-9025-4491-A63A-14CBE7CCDFC6}", AutoMap = true)]
    public class PageSessionExpiredModel : BaseSitecoreModel
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
        public virtual string MainText { get; set; }
    }
}
