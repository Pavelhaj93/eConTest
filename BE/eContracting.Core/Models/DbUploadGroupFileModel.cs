using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eContracting.Models
{
    /// <summary>
    /// Represents one upload field with multiple attached files.
    /// </summary>
    [Table("UploadGroups")]
    public class DbUploadGroupFileModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets unique group key.
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        [Required]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the output single file optimized from <see cref="OriginalFiles"/>.
        /// </summary>
        [Required]
        public DbFileModel OutputFile { get; set; }

        /// <summary>
        /// Gets or sets original uploaded files.
        /// </summary>
        [Required]
        public virtual ICollection<DbFileModel> OriginalFiles { get; set; }
    }
}
