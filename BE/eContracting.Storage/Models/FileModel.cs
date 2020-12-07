using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Storage.Models
{
    [Table("Files")]
    public class FileModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets unique key.
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the file name without extension.
        /// </summary>
        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileExtension { get; set; }

        [Required]
        public string MimeType { get; set; }

        [Required]
        public int Size { get; set; }

        [Required]
        public byte[] Content { get; set; }
    }
}
