using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class UploadGroupFileOperationResultModel
    {
        public DbUploadGroupFileModel DbUploadGroupFileModel { get; set; }

        public bool IsSuccess { get; set; } = false;

        public ErrorModel ErrorModel { get; set; }
    }
}
