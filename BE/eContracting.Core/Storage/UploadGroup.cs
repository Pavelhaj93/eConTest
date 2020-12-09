namespace eContracting.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UploadGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UploadGroup()
        {
            UploadGroupOriginalFiles = new HashSet<UploadGroupOriginalFile>();
        }

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

        public virtual File File { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UploadGroupOriginalFile> UploadGroupOriginalFiles { get; set; }
    }
}
