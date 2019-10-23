// <copyright file="AuthenticationModel.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AuthenticationModel
    {
        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Additional { get; set; }

        [Required]
        public string ItemValue { get; set; }

        [Required]
        public bool IsUserChoice { get; set; }

        [Required]
        public SelectList AvailableFields { get;set;}

        [Required]
        public string SelectedKey { get; set; }
    }
}
