using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;

namespace eContracting
{
    public interface ISitecoreContextExtended : ISitecoreContext
    {
        /// <summary>
        /// Gets children of <typeparamref name="T"/> from folder <paramref name="path"/>.
        /// </summary>
        /// <typeparam name="T">Sitecore item model type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="path">The path.</param>
        /// <returns>List of found children or empty list.</returns>
        IEnumerable<T> GetItems<T>(string path);
    }
}
