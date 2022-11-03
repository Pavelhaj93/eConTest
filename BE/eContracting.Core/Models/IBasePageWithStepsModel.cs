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
    /// Represents common page with <see cref="Step_Default"/>.
    /// </summary>
    /// <seealso cref="eContracting.Models.IBasePageModel" />
    public interface IBasePageWithStepsModel : IBasePageModel
    {
        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        [SitecoreField]
        IStepModel Step_Default { get; set; }

        [SitecoreField]
        IStepModel Step_NoLogin { get; set; }

        [SitecoreField]
        IStepModel Step_DistributorChange { get; set; }
        
        [SitecoreField]
        IStepModel Step_ProductChange { get; set; }
    }
}
