using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glass.Mapper.Sc;

namespace eContracting
{
    public interface ISitecoreServiceExtended : ISitecoreService
    {
        /// <summary>
        /// Gets children of <typeparamref name="T"/> from folder <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// Because unit test cannot work with extension methods, this substitutes extensino method for 'GetItems{T}'.
        /// BUT you have to also mock 'GetItem{T}' because unit test will go to this method. Otherwise everytime 'GetItems{T}' returns null!.
        /// </remarks>
        /// <typeparam name="T">Sitecore item model type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="path">The path.</param>
        /// <returns>List of found children or empty list.</returns>
        IEnumerable<T> GetItems<T>(string path);
    }
}
