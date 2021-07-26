using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{978F7ACC-208E-4926-8313-FB8218500B22}", AutoMap = true)]
    public interface IPromoBoxModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string Text { get; set; }

        [SitecoreField]
        IColorModel Color { get; set; }

        [SitecoreField]
        Link Link { get; set; }
    }
}
