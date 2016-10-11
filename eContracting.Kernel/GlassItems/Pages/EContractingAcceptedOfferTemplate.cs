using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{442E0DAA-1D1B-49D3-B4E8-38B35ABB6AEC}", AutoMap = true)]
    public class EContractingAcceptedOfferTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string MainText { get; set; }
    }
}
