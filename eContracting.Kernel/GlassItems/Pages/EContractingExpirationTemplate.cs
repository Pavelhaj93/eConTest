using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{22D897AA-70B8-436F-8609-53898C556353}", AutoMap = true)]
    public class EContractingExpirationTemplate : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

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
