using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{89052C3A-8D1D-427E-9F4C-24FB58DE21CE}", AutoMap = true)]
    public class EContractingOfferTemplate : EContractingTemplate
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link NextPageUrl { get; set; }
    }
}
