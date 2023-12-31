namespace eContracting.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileAttribute
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
