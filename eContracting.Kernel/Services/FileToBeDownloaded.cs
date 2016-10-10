using System;
using System.Collections.Generic;

namespace eContracting.Kernel.Services
{
    [Serializable]
    public class FileToBeDownloaded
    {
        public string Index { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public List<Byte> FileContent { get; set; }
    }

}
