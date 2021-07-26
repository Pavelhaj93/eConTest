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
    /// Represents common page with <see cref="PageTitle"/>.
    /// </summary>
    /// <seealso cref="eContracting.Models.IBaseSitecoreModel" />
    public interface IBasePageModel : IBaseSitecoreModel
    {
        /// <summary>
        /// Gets or sets the page title.
        /// </summary>
        [SitecoreField]
        string PageTitle { get; set; }
    }
}
