using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Represents file from 'ZCCH_CACHE_API'
    /// </summary>
    [Serializable]
    public class AttachmentModel
    {
        /// <summary>
        /// Gets or set index.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets file number.
        /// </summary>
        public string FileNumber { get; set; }

        /// <summary>
        /// Gets or sets a file type.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether signing is required.
        /// </summary>
        public bool SignRequired { get; set; }

        /// <summary>
        /// Gets or sets a template alc id.
        /// </summary>
        public string TemplAlcId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this version of document is signed.
        /// </summary>
        public bool SignedVersion { get; set; }

        /// <summary>
        /// Gets or sets file content.
        /// </summary>
        public List<Byte> FileContent { get; set; }
    }
}
