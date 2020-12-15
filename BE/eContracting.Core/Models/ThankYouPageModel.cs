using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    /// <summary>
    /// Represents model for thank you page.
    /// </summary>
    /// <seealso cref="BasePageWithStepsModel" />
    [SitecoreType(TemplateId = "{4F3B54BA-3DBC-408C-9573-F9F86AC0C3C7}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class ThankYouPageModel : BasePageWithStepsModel
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link1 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link2 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link3 { get; set; }
    }
}
