using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class UploadGroup
    {
        public UploadGroup(DbUploadGroupFileModel model) : this()
        {
            this.Id = model.Id;
            this.Key = model.Key;
            this.Guid = model.Guid;
            this.SessionId = model.SessionId;

            if (model.OutputFile != null)
            {
                this.File = new File(model.OutputFile);
            }

            if (model.OriginalFiles?.Length > 0)
            {
                this.UploadGroupOriginalFiles = model.OriginalFiles.Select(x => new UploadGroupOriginalFile(x) { GroupId = this.Id, UploadGroup = this }).ToArray();
            }
        }

        public DbUploadGroupFileModel ToModel()
        {
            var model = new DbUploadGroupFileModel();
            model.Id = this.Id;
            model.Key = this.Key;
            model.Guid = this.Guid;
            model.SessionId = this.SessionId;
            model.OutputFile = this.File?.ToModel();

            if (this.UploadGroupOriginalFiles?.Count > 0)
            {
                model.OriginalFiles = this.UploadGroupOriginalFiles.Where(x => x.File != null).Select(x => x.File.ToModel()).ToArray();
            }

            return model;
        }
    }
}
