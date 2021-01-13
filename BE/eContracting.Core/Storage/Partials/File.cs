using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class File
    {
        [NotMapped]
        public DbFileModel Model { get; set; }

        public File()
        {
        }

        public File(DbFileModel model)
        {
            this.Model = model;
            this.Id = model.Id;
            this.FileName = model.FileName;
            this.FileExtension = model.FileExtension;
            this.MimeType = model.MimeType;
            this.Size = model.Size;
            this.Content = model.Content;
        }

        public DbFileModel ToModel(IEnumerable<FileAttribute> attributes)
        {
            var model = new DbFileModel();
            model.Id = this.Id;
            model.FileName = this.FileName;
            model.FileExtension = this.FileExtension;
            model.MimeType = this.MimeType;
            model.Content = this.Content;
            model.Attributes.AddRange(attributes.Select(x => x.ToModel()));
            return model;
        }
    }
}
