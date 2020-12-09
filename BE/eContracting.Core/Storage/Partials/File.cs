using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class File
    {
        public File(DbFileModel model)
        {
            this.Id = model.Id;
            //this.Key = model.Key;
            this.FileName = model.FileName;
            this.FileExtension = model.FileExtension;
            this.MimeType = model.MimeType;
            this.Size = model.Size;
            this.Content = model.Content;

            if (model.Attributes?.Length > 0)
            {
                this.FileAttributes = model.Attributes.Select(x => new FileAttribute(x)).ToArray();
            }
        }

        public DbFileModel ToModel()
        {
            var model = new DbFileModel();
            model.Id = this.Id;
            model.FileName = this.FileName;
            model.FileExtension = this.FileExtension;
            //model.Key = this.Key
            model.MimeType = this.MimeType;
            model.Size = this.Size;
            model.Content = this.Content;

            if (this.FileAttributes?.Count > 0)
            {
                model.Attributes = this.FileAttributes.Select(x => x.ToModel()).ToArray();
            }

            return model;
        }
    }
}
