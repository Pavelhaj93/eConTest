// <copyright file="AuthenticationModel.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Models
{
    using System.ComponentModel.DataAnnotations;

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
