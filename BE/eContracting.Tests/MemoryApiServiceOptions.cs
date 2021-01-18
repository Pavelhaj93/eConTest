using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Tests
{
    public class MemoryApiServiceOptions : BaseApiServiceOptions
    {
        public MemoryApiServiceOptions(string url, string base64Username, string base64password) : base(url, base64Username, base64password)
        {
        }
    }
}
