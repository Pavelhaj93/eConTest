using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{94085F0E-BF9D-4174-8A97-17ACF4A74E71}", AutoMap = true)]
    public interface IInfoBoxModel : IBaseSitecoreModel
    {
        [SitecoreField]
        string Title { get; set; }

        [SitecoreField]
        Link ButtonUrl { get; set; }

        [SitecoreField]
        string ButtonLabel { get; set; }

        [SitecoreChildren]
        IEnumerable<IInfoBoxItemModel> Items { get; set; }
    }
}
