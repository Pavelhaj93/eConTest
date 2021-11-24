using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{65303073-7AE5-450B-A8B4-722893F6BF86}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class ListItemModel
    {
        [SitecoreField]
        public virtual string Text { get; set; }

        [SitecoreField]
        public virtual string Value { get; set; }
    }
}
