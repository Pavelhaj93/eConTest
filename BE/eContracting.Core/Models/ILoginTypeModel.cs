using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{1CF3E1F2-7C49-4ED6-9CB1-FA9EF95A73DC}", AutoMap = true)]
    public interface ILoginTypeModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string Label { get; set; }

        [SitecoreField]
        string Key { get; set; }

        [SitecoreField]
        string HelpText { get; set; }

        [SitecoreField]
        string Placeholder { get; set; }

        [SitecoreField]
        string ValidationRegex { get; set; }

        [SitecoreField]
        string ValidationMessage { get; set; }
    }
}
