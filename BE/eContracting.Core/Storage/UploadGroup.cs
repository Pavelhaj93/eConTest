namespace eContracting.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UploadGroup
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Key { get; set; }

        [Required]
        [StringLength(255)]
        public string SessionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Guid { get; set; }

        public int OutputFileId { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
