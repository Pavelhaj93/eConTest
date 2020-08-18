using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Kernel.Models
{
    public class AuthenticationSettingsModel
    {
        public IEnumerable<AuthenticationSettingsItemModel> AuthFields { get; set; }
    }
}
