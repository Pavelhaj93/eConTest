using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models.JsonDescriptor
{
    public class ContainerModel
    {
        public List<IDataModel> Data { get; } = new List<IDataModel>();
    }
}
