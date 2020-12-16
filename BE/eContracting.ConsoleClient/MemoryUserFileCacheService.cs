using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Storage;

namespace eContracting.ConsoleClient
{
    class MemoryUserFileCacheService : IUserFileCacheService
    {
        public void Clear(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task ClearAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task<DbUploadGroupFileModel> FindGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task<DbSignedFileModel> FindSignedFileAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task RemoveGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task RemoveSignedFileAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(DbUploadGroupFileModel group)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(DbSignedFileModel file)
        {
            throw new NotImplementedException();
        }
    }
}
