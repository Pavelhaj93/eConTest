﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Storage.Models
{
    [Table("UploadGroups")]
    public class UploadGroupFileModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public FileModel OutputFile { get; set; }

        [Required]
        public virtual ICollection<FileModel> UploadedFiles { get; set; }

        [Required]
        public virtual ICollection<FileModel> OptimizedFiles { get; set; }
    }
}
