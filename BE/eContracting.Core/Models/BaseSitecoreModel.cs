using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    public abstract class BaseSitecoreModel
    {
        [SitecoreId]
        public virtual Guid ID { get; set; }

        [SitecoreInfo(Type = SitecoreInfoType.Name)]
        public virtual string Name { get; set; }
    }
}
