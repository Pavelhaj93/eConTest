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
    [ExcludeFromCodeCoverage]
    public class ProcessTypeModel : BaseSitecoreModel
    {
        [SitecoreField]
        public virtual string Title { get; set; }

        [SitecoreField]
        public virtual string Code { get; set; }
    }
}
