using System;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{59CCBDB6-6AFE-4630-8EAD-04C42F599B14}", AutoMap = true)]
    public class EContractingUserBlockedTemplate : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string Header { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }
    }
}
