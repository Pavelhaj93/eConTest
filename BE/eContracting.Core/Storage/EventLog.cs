namespace eContracting.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EventLog
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [StringLength(255)]
        public string SessionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Guid { get; set; }

        [Required]
        [StringLength(50)]
        public string Event { get; set; }

        public string Message { get; set; }

        public string Error { get; set; }
    }
}
