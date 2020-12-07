using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class FileModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string MimeType { get; set; }

        public string Extension { get; set; }

        public byte[] Content { get; set; }
    }
}
