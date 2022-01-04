using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{1C38E7D9-42D9-4C95-9800-89E0069037BA}", AutoMap = true)]
    public interface IPageFooterModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string CopyrightText { get; set; }

        [SitecoreField]
        string EmailText { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        string EmailLink { get; set; }

        [SitecoreField]
        string PhoneNumberFooter { get; set; }

        [SitecoreField]
        string PhoneNumberLinkFooter { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link DisclaimerLink { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link CookiePolicyLink { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link CookieSettingsLink { get; set; }
    }
}
