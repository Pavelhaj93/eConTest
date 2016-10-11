using System;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{F616CC8F-B16F-4E9E-9EB2-6875FDEAE5AA}", AutoMap = true)]
    public class EContracting404Template : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField]
        public virtual string HeaderText { get; set; }

        [SitecoreField]
        public virtual string Text { get; set; }
    }
}
