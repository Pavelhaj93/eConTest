using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

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

        /// <summary>
        /// Tries to find text by give <paramref name="key"/> and try to replace tokens from <paramref name="replaceTokens"/> collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="replaceTokens">The replace tokens collection. <see cref="KeyValuePair.Key"/> is token, <see cref="KeyValuePair.Value"/> is replacement.</param>
        /// <returns></returns>
        string FindByKey(string key, IDictionary<string, string> replaceTokens);

        /// <summary>
        /// Translate error code to readable message.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>Rich message or <paramref name="code"/>.</returns>
        string ErrorCode(string code);

        string Error(ErrorModel error);
    }
}
