using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Sitecore.Globalization;

namespace eContracting.Services
{
    public class SitecoreTextService : ITextService
    {
        public string Error(ErrorModel error)
        {
            var translation = Translate.Text(error.Code);

            if (translation != error.Code)
            {
                return translation;
            }

            if (!string.IsNullOrEmpty(error.Description))
            {
                return error.Description;
            }

            return error.Code;
        }

        /// <inheritdoc/>
        public string ErrorCode(string code)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string FindByKey(string key)
        {
            return Translate.Text(key);
        }

        /// <inheritdoc/>
        public string FindByKey(string key, IDictionary<string, string> replaceTokens)
        {
            var text = this.FindByKey(key);

            if (replaceTokens.Any())
            {
                foreach (var item in replaceTokens)
                {
                    text = text.Replace("{" + item.Key + "}", item.Value);
                }
            }

            return text;
        }
    }
}
