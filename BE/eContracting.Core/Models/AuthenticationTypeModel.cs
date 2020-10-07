using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class AuthenticationTypeModel
    {
        public string Label { get; set; }
        public string Key { get; set; }
        public string HelpText { get; set; }
        public string Placeholder { get; set; }
        public bool EnableForDefault { get; set; }
        public bool EnableForRetention { get; set; }
        public bool EnableForAcquisition { get; set; }
        public string ValidationRegex { get; set; }
        public string ValidationMessage { get; set; }
    }
}
