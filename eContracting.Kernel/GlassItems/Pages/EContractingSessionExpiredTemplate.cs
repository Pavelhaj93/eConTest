using System;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{9FE07BF2-93BA-4FB0-954E-0F08E5E9AF7B}", AutoMap = true)]
    public class EContractingSessionExpiredTemplate : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string Header { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }
    }
}
