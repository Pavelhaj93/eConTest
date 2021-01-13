namespace eContracting.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LoginAttempt
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        [Required]
        [StringLength(255)]
        public string SessionId { get; set; }

        [Required]
        [StringLength(255)]
        public string Guid { get; set; }

        public short WrongBirthdate { get; set; }

        public short WrongValue { get; set; }
    }
}
