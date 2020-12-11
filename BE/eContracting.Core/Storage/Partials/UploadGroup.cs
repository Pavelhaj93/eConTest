using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class UploadGroup
    {
        [NotMapped]
        public DbUploadGroupFileModel Model { get; set; }

        public UploadGroup()
        {
        }

        public UploadGroup(DbUploadGroupFileModel model)
        {
            this.Model = model;
            this.Id = model.Id;
            this.Key = model.Key;
            this.Guid = model.Guid;
            this.SessionId = model.SessionId;
        }

        public DbUploadGroupFileModel ToModel()
        {
            var model = new DbUploadGroupFileModel();
            model.Id = this.Id;
            model.Key = this.Key;
            model.Guid = this.Guid;
            model.SessionId = this.SessionId;
            return model;
        }
    }
}
