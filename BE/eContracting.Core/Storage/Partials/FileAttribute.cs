using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class FileAttribute
    {
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
