using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Storage;

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
        public Task<DbUploadGroupFileModel> FindGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<DbSignedFileModel> FindSignedFileAsync(DbSearchParameters search)
        {
            using (var db = new DatabaseContext(this.ConnectionString))
            {
                var query = db.SignedFiles.AsQueryable();
                query = query.Where(x => x.Key == search.Key);
                query = query.Where(x => x.Guid == search.Guid);
                query = query.Where(x => x.SessionId == search.SessionId);
                var result = await query.FirstOrDefaultAsync();
                return result.ToModel();
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
        public async Task SetAsync(DbSignedFileModel model)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var dbModel = context.SignedFiles.FirstOrDefault(x => x.Key == model.Key && x.Guid == model.Guid && x.SessionId == model.SessionId);

                if (dbModel != null)
                {
                    dbModel.File.Content = model.File.Content;
                    var entry = context.Entry(dbModel.File);
                    entry.State = System.Data.Entity.EntityState.Modified;
                    var result = await context.SaveChangesAsync();
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var file = new File();
                        file.FileName = model.File.FileName;
                        file.FileExtension = model.File.FileExtension;
                        file.MimeType = model.File.MimeType;
                        file.Size = model.File.Size;
                        file.Content = model.File.Content;

                        var newFile = context.Files.Add(file);

                        var fileAttributes = new List<FileAttribute>();

                        if (model.File.Attributes?.Length > 0)
                        {
                            for (int i = 0; i < model.File.Attributes.Length; i++)
                            {
                                fileAttributes.Add(new FileAttribute(model.File.Attributes[i]) { FileId = newFile.Id });
                            }
                        }

                        var newAttributes = context.FileAttributes.AddRange(fileAttributes);

                        var signedFile = new SignedFile();
                        signedFile.Guid = model.Guid;
                        signedFile.Key = model.Key;
                        signedFile.SessionId = model.SessionId;
                        signedFile.FileId = newFile.Id;

                        context.SignedFiles.Add(signedFile);
                        var result = await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
