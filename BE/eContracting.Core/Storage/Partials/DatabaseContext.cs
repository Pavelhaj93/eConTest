using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Storage
{
    public partial class DatabaseContext
    {
        public DatabaseContext(string connectionString) : base(connectionString)
        {
        }
    }
}
