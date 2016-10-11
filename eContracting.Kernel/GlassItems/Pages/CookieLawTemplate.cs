using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{725B637D-FA68-4FC4-BA8E-1BCAF62A78DB}", AutoMap = true)]
    public class CookieLawTemplate : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link Link { get; set; }

        [SitecoreField]
        public virtual string ButtonText { get; set; }
    }
}
