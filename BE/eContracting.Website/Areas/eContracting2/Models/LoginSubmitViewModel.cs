using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class LoginSubmitViewModel
    {
        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
