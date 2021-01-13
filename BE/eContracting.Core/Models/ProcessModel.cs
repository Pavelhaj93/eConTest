using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{20EC5C2E-616E-493A-892D-C0C1E23721E9}", AutoMap = true)]
    [ExcludeFromCodeCoverage]
    public class ProcessModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string Title { get; set; }

        [SitecoreField]
        public virtual string Code { get; set; }
    }
}
