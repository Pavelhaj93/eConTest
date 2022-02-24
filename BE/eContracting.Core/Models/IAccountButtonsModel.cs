using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{14111589-96E4-449E-AE4A-3ED3EFB48480}", AutoMap = true)]
    public interface IAccountButtonsModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string ButtonNewAccountLabel { get; set; }

        [SitecoreField]
        string ButtonLoginAccountLabel { get; set; }

        [SitecoreField]
        string ButtonLoginAccountTooltip { get; set; }

        [SitecoreField]
        string ButtonDashboardLabel { get; set; }

        [SitecoreField(UrlOptions = SitecoreInfoUrlOptions.AlwaysIncludeServerUrl)]
        Link ButtonDashboardUrl { get; set; }
    }
}
