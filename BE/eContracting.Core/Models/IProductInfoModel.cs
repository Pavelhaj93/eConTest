using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{7E546BC0-457A-4E17-B1A8-28B71B586FA8}", AutoMap = true)]
    public interface IProductInfoModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string Key { get; set; }

        [SitecoreField]
        string Note { get; set; }

        [SitecoreField("XML_attributes")]
        NameValueCollection XmlAttributes { get; set; }

        [SitecoreField("XML_attributes")]
        string XmlAttributesRaw { get; set; }
    }
}
