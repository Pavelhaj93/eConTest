using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models.JsonDescriptor
{
    public class BenefitDataModel : IDataModel
    {
        public string Type { get; } = "benefit";
        public int Position { get; set; }
        public IDataHeaderModel Header { get; set; }
        public IDataBodyModel Body { get; set; }
    }
}
