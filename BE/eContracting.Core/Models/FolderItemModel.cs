using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    [ExcludeFromCodeCoverage]
    public class FolderItemModel<TChild> : BaseSitecoreModel
    {
        [SitecoreChildren]
        public virtual IEnumerable<TChild> Children { get; set; }
    }
}
