using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class AuthResultModel
    {
        public bool IsCorrectBirthday { get; set; }

        public bool IsCorrentValue { get; set; }

        public bool Succeeded { get; set; }
    }
}
