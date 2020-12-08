using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eContracting.Models
{
    /// <summary>
    /// Represents additional file attributes serialized by its purpose.
    /// </summary>
    public class DbFileAttributeModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        public DbFileModel File { get; set; }

        //[ForeignKey("File")]
        public int FileId { get; set; }

        public DbFileAttributeModel()
        {
        }

        public DbFileAttributeModel(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
