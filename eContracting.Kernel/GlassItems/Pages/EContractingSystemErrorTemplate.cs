using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{F2B5941C-CA39-4965-A969-063A84EDB360}", AutoMap = true)]
    public class EContractingSystemErrorTemplate : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string Header { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link1 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link2 { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link3 { get; set; }
    }
}
