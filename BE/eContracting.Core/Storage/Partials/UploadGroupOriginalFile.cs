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

        public UploadGroupOriginalFile(DbFileModel model)
        {
            this.FileId = model.Id;
            this.File = new File(model);
        }

        public DbFileModel ToModel()
        {
            return this.File?.ToModel();
        }
    }
}
