using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{05A8A6B2-D3D7-41E5-B9B7-8CB8CBE39488}", AutoMap = true)]
    public class EContractingTemplate
    {
        [SitecoreField]
        public virtual string PageTitle { get; set; }
    }
}
