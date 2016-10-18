using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.GlassItems.Pages
{
    [SitecoreType(TemplateId = "{0EEAE94D-0018-40C8-A537-5B67349985CD}", AutoMap = true)]
    public class EContractingAuthenticationTemplate : EContractingTemplate
    {
        [SitecoreField]
        public virtual string DateOfBirth { get; set; }

        [SitecoreField]
        public virtual string ContractData { get; set; }

        [SitecoreField]
        public virtual string ButtonText { get; set; }

        [SitecoreField]
        public virtual string MainText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link NextPageLink { get; set; }
    }
}
