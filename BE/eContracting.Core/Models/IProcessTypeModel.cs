using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{CD3F44A2-6A63-4631-980C-8AB0BFE2A33F}", AutoMap = true)]
    public interface IProcessTypeModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string Title { get; set; }

        [SitecoreField]
        string Code { get; set; }

        [SitecoreField("eLab")]
        string GoogleAnalytics_eLab { get; set; }

        [SitecoreField("e_Name")]
        string e_Name { get; set; }
    }
}
