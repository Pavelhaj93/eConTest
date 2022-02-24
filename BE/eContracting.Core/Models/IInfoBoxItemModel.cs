using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{3FC99AA2-F813-4560-B602-A6196BF1CB78}", AutoMap = true)]
    public interface IInfoBoxItemModel
    {
        [SitecoreField]
        string Text { get; set; }
    }
}
