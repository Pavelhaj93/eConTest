using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.ConsoleClient
{
    public class MemoryProcessModel : IProcessModel
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string GoogleAnalytics_eAct { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public string e_Name { get; set; }
        public Guid TemplateId { get; set; }
    }
}
