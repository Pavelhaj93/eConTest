using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    public class DbUserFileCacheService : IUserFileCacheService
    {
        protected readonly string ConnectionString;

        public DbUserFileCacheService(ISettingsReaderService settingsReader)
        {
            this.ConnectionString = settingsReader.GetFileCacheStorageConnectionString();
        }

        /// <inheritdoc/>
        public Task ClearAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<DbUploadGroupFileModel> GetGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<DbSignedFileModel> GetSignedFileAsync(DbSearchParameters search)
        {
            using (var db = new DbContext(this.ConnectionString))
            {
                var query = db.SignedFiles.AsQueryable();
                query = query.Where(x => x.Key == search.Key);
                query = query.Where(x => x.Guid == search.Guid);
                query = query.Where(x => x.SessionId == search.SessionId);
                var result = await query.FirstOrDefaultAsync();
                return result;
            }
        }

        /// <inheritdoc/>
        public Task RemoveGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task RemoveSignedFileAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SetAsync(DbUploadGroupFileModel group)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task SetAsync(DbSignedFileModel file)
        {
            using (var db = new DbContext(this.ConnectionString))
            {
                var dbModel = db.SignedFiles.FirstOrDefault(x => x.Key == file.Key && x.Guid == file.Guid && x.SessionId == file.SessionId);

                if (dbModel != null)
                {
                    dbModel.File.Content = file.File.Content;
                    var entry = db.Entry(dbModel.File);
                    entry.State = System.Data.Entity.EntityState.Modified;
                    var result = await db.SaveChangesAsync();
                }
                else
                {
                    db.SignedFiles.Add(file);
                    var result = await db.SaveChangesAsync();
                }
            }
        }
    }
}
