using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    public abstract class BaseSitecoreModel : IBaseSitecoreModel
    {
        [SitecoreId]
        public Guid ID { get; set; }

        [SitecoreInfo(Type = SitecoreInfoType.Name)]
        public string Name { get; set; }

        [SitecoreInfo(Type = SitecoreInfoType.DisplayName)]
        public string DisplayName { get; set; }

        [SitecoreInfo(Type = SitecoreInfoType.FullPath)]
        public string Path { get; set; }
    }
}
