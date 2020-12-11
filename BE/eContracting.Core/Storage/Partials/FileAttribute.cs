using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class FileAttribute
    {
        [NotMapped]
        public DbFileAttributeModel Model { get; set; }

        public FileAttribute()
        {
        }

        public FileAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public FileAttribute(DbFileAttributeModel model)
        {
            this.Model = model;
            this.Id = model.Id;
            this.Name = model.Name;
            this.Value = model.Value;
        }

        public DbFileAttributeModel ToModel()
        {
            var model = new DbFileAttributeModel();
            model.Id = this.Id;
            model.FileId = this.FileId;
            model.Name = this.Name;
            model.Value = this.Value;
            return model;
        }
    }
}
