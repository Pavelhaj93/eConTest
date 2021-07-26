using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [SitecoreType(AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.No)]
    public interface IFolderItemModel<TChild> : IBaseSitecoreModel where TChild : IBaseSitecoreModel
    {
        [SitecoreChildren(EnforceTemplate = SitecoreEnforceTemplate.No)]
        IEnumerable<TChild> Children { get; set; }
    }
}
