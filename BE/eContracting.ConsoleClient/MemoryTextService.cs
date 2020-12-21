using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.ConsoleClient
{
    class MemoryTextService : ITextService
    {
        public string FindByKey(string key)
        {
            return key;
        }
    }
}
