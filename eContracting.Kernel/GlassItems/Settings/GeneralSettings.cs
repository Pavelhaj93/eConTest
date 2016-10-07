using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Kernel.GlassItems.Settings
{
    [SitecoreType(TemplateId = "{0D513A12-13CC-48F1-BD97-58109010109A}", AutoMap = true)]
    public class GeneralSettings
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link SessionExpired { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link UserBlocked { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link AcceptedOffer { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link WrongUrl { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        public virtual Link OfferExpired { get; set; }
    }
}
