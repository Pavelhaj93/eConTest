using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class UploadGroupOriginalFile
    {
        public UploadGroupOriginalFile()
        {
        }

        public UploadGroupOriginalFile(DbUploadGroupFileModel group, DbFileModel model)
        {
            this.GroupId = group.Id;
            this.FileId = model.Id;
        }
    }
}
