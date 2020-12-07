using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eContracting.Models
{
    /// <summary>
    /// Represents additional file attributes serialized by its purpose.
    /// </summary>
    [Table("FileAttributes")]
    public class DbFileAttributeModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

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
