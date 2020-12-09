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
    /// Represents common page with <see cref="Step"/>.
    /// </summary>
    /// <seealso cref="eContracting.Models.BasePageModel" />
    [ExcludeFromCodeCoverage]
    public abstract class BasePageWithStepsModel : BasePageModel
    {
        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        [SitecoreField]
        public virtual ProcessStepModel Step { get; set; }
    }
}
