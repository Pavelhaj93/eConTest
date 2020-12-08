using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eContracting.Models
{
    /// <summary>
    /// Represents physical file.
    /// </summary>
    [Table("Files")]
    public class DbFileModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the file name without extension.
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        [Required]
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of the file.
        /// </summary>
        [Required]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets file size.
        /// </summary>
        [Required]
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets file content in bytes.
        /// </summary>
        [Required]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets additional file attributes.
        /// </summary>
        public virtual ICollection<DbFileAttributeModel> Attributes { get; set; }

        public DbFileModel()
        {
            this.Attributes = new HashSet<DbFileAttributeModel>();
        }
    }
}
