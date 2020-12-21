using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    public interface ITextService
    {
        /// <summary>
        /// Tries to find text by give <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Found text or empty string.</returns>
        string FindByKey(string key);
    }
}
