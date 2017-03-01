using System.ComponentModel.DataAnnotations;

namespace eContracting.Kernel.Models
{
    public class AuthenticationModel
    {
        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Additional { get; set; }

        [Required]
        public string ItemValue { get; set; }
    }
}
