using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Initialize the code in <see cref="IStartup.Initialize"/> method when Sitecore stars (initialize pipeline).
    /// </summary>
    /// <remarks>
    /// Do not use DI because something (like ISitecoreContext) won't be available in initialize pipeline.
    /// </remarks>
    public interface IStartup
    {
        /// <summary>
        /// Initialize the code.
        /// </summary>
        void Initialize();
    }
}
