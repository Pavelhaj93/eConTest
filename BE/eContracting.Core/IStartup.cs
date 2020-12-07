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
    public interface IStartup
    {
        /// <summary>
        /// Initialize the code.
        /// </summary>
        void Initialize();
    }
}
