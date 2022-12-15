using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Storage;

namespace eContracting.Models
{
    public class CallMeBackModel
    {
        public string OfferGuid { get; set; }

        public string Description { get; set; }

        public string Process { get; set; }

        public string ProcessType { get; set; }

        public string ExternalSystem { get; set; }

        public string Phone { get; set; }

        public string SelectedTime { get; set; }

        public string Note { get; set; }

        public string Partner { get; set; }

        public string EicEan { get; set; }

        public File[] Files { get; set; }

        public string[] AllowedExtensions { get; set; }

        public string RequestedUrl { get; set; }

        /// <summary>
        /// Gets array of all errors collected for this CMB.
        /// </summary>
        public IList<string> Errors { get; } = new List<string>();

        public class File
        {
            public string Name { get; set; }

            public string MimeType { get; set; }

            public byte[] Content { get; set; }
        }
    }
}
