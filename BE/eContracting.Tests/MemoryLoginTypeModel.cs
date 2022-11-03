using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public class MemoryLoginTypeModel : ILoginTypeModel
    {
        public string Label { get; set; }
        public string Key { get; set; }
        public string HelpText { get; set; }
        public string Placeholder { get; set; }
        public string ValidationRegex { get; set; }
        public string ValidationMessage { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public Guid TemplateId { get; set; }
    }
}
