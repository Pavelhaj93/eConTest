using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace eContracting.Models
{
    /// <summary>
    /// Represent container for all definition including data for default values.
    /// </summary>
    /// <seealso cref="eContracting.Models.IDefinitionCombinationDataModel" />
    [SitecoreType(TemplateId = "{429BA55F-E51F-4F70-8473-2B33301B25AE}", AutoMap = true)]
    public interface IDefinitionCombinationContainerModel : IDefinitionCombinationDataModel
    {
    }
}
