using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eContracting.Models
{
    /// <summary>
    /// Represents one upload field with multiple attached files.
    /// </summary>
    public class DbUploadGroupFileModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets unique group key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets offer unique identifier.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the output single file optimized from <see cref="OriginalFiles"/>.
        /// </summary>
        public DbFileModel OutputFile { get; set; }

        /// <summary>
        /// Gets or sets original uploaded files.
        /// </summary>
        public virtual List<DbFileModel> OriginalFiles { get; } = new List<DbFileModel>();
    }
}
