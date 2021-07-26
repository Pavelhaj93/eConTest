using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public class MemoryFolderItemModel<T> : IFolderItemModel<T> where T : IBaseSitecoreModel
    {
        public IEnumerable<T> Children { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }

        public MemoryFolderItemModel()
        {
        }

        public MemoryFolderItemModel(IEnumerable<T> items)
        {
            this.Children = items;
        }
    }
}
