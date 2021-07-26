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
    [SitecoreType(TemplateId = "{B7AA8D70-22CB-47B8-87B4-F070810D168D}", AutoMap = true)]
    public interface IPageHeaderModel : IBaseSitecoreModel
    {
        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link ImageLink { get; set; }

        [SitecoreField]
        string PhoneNumber { get; set; }

        [SitecoreField]
        string PhoneNumberLink { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link LogoutLink { get; set; }
    }
}
