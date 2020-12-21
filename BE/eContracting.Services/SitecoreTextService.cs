using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Globalization;

namespace eContracting.Services
{
    public class SitecoreTextService : ITextService
    {
        /// <inheritdoc/>
        public string FindByKey(string key)
        {
            return Translate.Text(key);
        }
    }
}
