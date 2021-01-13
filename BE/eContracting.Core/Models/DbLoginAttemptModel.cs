using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class DbLoginAttemptModel
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        public string Guid { get; set; }

        public string SessionId { get; set; }

        public int WrongBirthdayDateCount { get; set; }

        public int WrongPostalCodeCount { get; set; }

        public int WrongResidencePostalCodeCount { get; set; }

        public int WrongPartnerNumberCount { get; set; }

        public int GeneralErrorCount { get; set; }

        public int SuccessAttemptCount { get; set; }
    }
}
