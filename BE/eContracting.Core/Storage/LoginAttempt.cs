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
        [Index(IsUnique =false)]
        public string Guid { get; set; }

        public bool IsBirthdateValid { get; set; }

        [StringLength(255)]
        public string LoginTypeKey { get; set; }

        public bool IsLoginValueValid { get; set; }

        public string BrowserAgent { get; set; }

        [StringLength(255)]
        public string LoginState { get; set; }

        public bool IsBlocking { get; set; } = false;
    }
}
