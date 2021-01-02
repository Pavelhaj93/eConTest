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

        public DbUploadGroupFileModel FindGroup(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public DbSignedFileModel FindSignedFile(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public void RemoveGroup(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public void RemoveSignedFile(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public void Set(DbUploadGroupFileModel group)
        {
            throw new NotImplementedException();
        }

        public void Set(DbSignedFileModel file)
        {
            throw new NotImplementedException();
        }
    }
}
