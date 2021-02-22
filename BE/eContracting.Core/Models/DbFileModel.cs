using System;
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
        private int size = 0;
        byte[] content = null;

        /// <summary>
        /// Public constructor, sets the CreateDate
        /// </summary>
        public DbFileModel()
        {
            this.CreateDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

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
        public int Size
        {
            get { return (size == 0) ? this.Content?.Length ?? 0 : size;}
            private set { size = value; }
        }

        /// <summary>
        /// Gets or sets file content in bytes.
        /// </summary>
        public byte[] Content {
            get { return content; }
            set
            {
                content = value;
                this.Size = value?.Length ?? 0;
            }
        }

        /// <summary>
        /// Gets or sets datetime when the file was saved into our database
        /// </summary>
        public DateTime? CreateDate { get; set; }
        

        [NotMapped]
        public virtual List<DbFileAttributeModel> Attributes { get; } = new List<DbFileAttributeModel>();
    }
}
