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
        public readonly List<DbSignedFileModel> SignedFiles = new List<DbSignedFileModel>();

        public void Clear(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public DbUploadGroupFileModel FindGroup(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public List<DbUploadGroupFileModel> FindGroups(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public int GetTotalOutputFileSize(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        public DbSignedFileModel FindSignedFile(DbSearchParameters search)
        {
            return this.SignedFiles.FirstOrDefault(x => x.Key == search.Key);
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
