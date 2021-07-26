using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{0574008B-75BE-4C2D-8583-451A67D33E0B}", AutoMap = true)]
    public interface IRichTextModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string Text { get; set; }
    }
}
