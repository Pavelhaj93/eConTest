using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eContracting.Models
{
    /// <summary>
    /// Represents physical file.
    /// </summary>
    public class DbFileModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the file name without extension.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the file.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets file size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets file content in bytes.
        /// </summary>
        public byte[] Content { get; set; }

        public virtual DbFileAttributeModel[] Attributes { get; set; }
    }
}
