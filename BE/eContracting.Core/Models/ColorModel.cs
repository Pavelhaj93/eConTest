using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{88B6E827-31DB-4974-9A59-A63EDA322527}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class ColorModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string Value { get; set; }
    }
}
