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
            this.CreateDate = DateTime.UtcNow;
        }

        public UploadGroup(DbUploadGroupFileModel model)
        {
            this.Model = model;
            this.Id = model.Id;
            this.Key = model.Key;
            this.Guid = model.Guid;
            this.SessionId = model.SessionId;
            this.CreateDate = (model.CreateDate.HasValue)? model.CreateDate : DateTime.UtcNow;
        }

        public DbUploadGroupFileModel ToModel()
        {
            var model = new DbUploadGroupFileModel();
            model.Id = this.Id;
            model.Key = this.Key;
            model.Guid = this.Guid;
            model.SessionId = this.SessionId;
            model.CreateDate = this.CreateDate;
            return model;
        }
    }
}
