using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(TemplateId = "{9333CB51-1D24-4C67-90E8-FB229E92FBCE}", AutoMap = true)]
    public interface IListCollectionModel
    {
        [SitecoreChildren]
        IEnumerable<IListItemModel> Items { get; set; }
    }
}
