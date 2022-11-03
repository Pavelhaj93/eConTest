using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
namespace eContracting.Models
{
    /// <summary>
    /// Represents folder with defined steps.
    /// </summary>
    [SitecoreType(TemplateId = "{6D22F9B3-29FF-4568-8896-5B3CF78569A4}", AutoMap = true)]
    public interface IStepsModel : IBaseSitecoreModel
    {
        [SitecoreChildren]
        IEnumerable<IStepModel> Steps { get; set; }
    }
}
