using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.ConsoleClient
{
    class GlobalConfiguration
    {
        public string ServiceUrl { get; set; }
        public string ServiceUser { get; set; }
        public string ServicePassword { get; set; }

        public string ServiceSignUrl { get; set; }
        public string ServiceSignUser { get; set; }
        public string ServiceSignPassword { get; set; }
    }
}
