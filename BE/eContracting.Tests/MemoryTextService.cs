using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public class MemoryTextService : ITextService
    {
        public string Error(ErrorModel error)
        {
            return error.ToString();
        }

        public string ErrorCode(string code)
        {
            return code;
        }

        public string FindByKey(string key)
        {
            return key;
        }

        public string FindByKey(string key, IDictionary<string, string> replaceTokens)
        {
            return key;
        }
    }
}
