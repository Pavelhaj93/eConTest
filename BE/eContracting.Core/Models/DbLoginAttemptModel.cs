using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    [Table("LoginAttempts")]
    public class DbLoginAttemptModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public string Guid { get; set; }

        [Required]
        public string SessionId { get; set; }

        public int WrongBirthdayDateCount { get; set; }

        public int WrongPostalCodeCount { get; set; }

        public int WrongResidencePostalCodeCount { get; set; }

        public int WrongPartnerNumberCount { get; set; }

        public int GeneralErrorCount { get; set; }

        public int SuccessAttemptCount { get; set; }
    }
}
