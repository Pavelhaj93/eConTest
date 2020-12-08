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
        public Task ClearAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task<UploadGroup> GetGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public Task<SignedFile> GetSignedFileAsync(DbSearchParameters search)
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

        public Task SetAsync(UploadGroup group)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(SignedFile file)
        {
            throw new NotImplementedException();
        }
    }
}
