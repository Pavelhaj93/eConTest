using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{8BAD1560-472C-4327-8F05-EC886E539F76}", AutoMap = true)]
    public interface IProductInfoRootModel
    {
        [SitecoreField]
        string InfoGas { get; set; }

        [SitecoreField]
        string InfoElectricity { get; set; }
    }
}
