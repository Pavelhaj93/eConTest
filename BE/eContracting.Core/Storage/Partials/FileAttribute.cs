using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Storage
{
    public partial class FileAttribute
    {
        public FileAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
