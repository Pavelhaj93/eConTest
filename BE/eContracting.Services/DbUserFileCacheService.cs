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
    /// <summary>
    /// Database cache for user files.
    /// </summary>
    /// <seealso cref="eContracting.IUserFileCacheService" />
    public class DbUserFileCacheService : IUserFileCacheService
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbUserFileCacheService"/> class.
        /// </summary>
        /// <param name="settingsReader">The settings reader.</param>
        public DbUserFileCacheService(ISettingsReaderService settingsReader)
        {
            this.ConnectionString = settingsReader.GetFileCacheStorageConnectionString();
        }

        /// <inheritdoc/>
        public void Clear(DbSearchParameters search)
        {
            var task = Task.Run(() => this.ClearAsync(search));
            task.Wait();
        }

        /// <inheritdoc/>
        public async Task ClearAsync(DbSearchParameters search)
        {
            if (string.IsNullOrEmpty(search.Guid))
            {
                return;
            }

            using (var context = new DatabaseContext(this.ConnectionString))
            {
                bool changes = false;

                var signedFiles = context.SignedFiles.Where(x => x.Guid == search.Guid);

                if (signedFiles.Any())
                {
                    context.SignedFiles.RemoveRange(signedFiles);
                    changes = true;
                }

                var uploadGroups = context.UploadGroups.Where(x => x.Guid == search.Guid);

                if (signedFiles.Any())
                {
                    context.UploadGroups.RemoveRange(uploadGroups);
                    changes = true;
                }

                if (changes)
                {
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <inheritdoc/>
        public Task<DbUploadGroupFileModel> FindGroupAsync(DbSearchParameters search)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<DbSignedFileModel> FindSignedFileAsync(DbSearchParameters search)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var query = context.SignedFiles.AsQueryable();
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
                    dbModel = new SignedFile(model);
                    context.SignedFiles.Add(dbModel);
                    await context.SaveChangesAsync();

                    //using (var transaction = context.Database.BeginTransaction())
                    //{
                    //    var file = new File();
                    //    file.FileName = model.File.FileName;
                    //    file.FileExtension = model.File.FileExtension;
                    //    file.MimeType = model.File.MimeType;
                    //    file.Size = model.File.Size;
                    //    file.Content = model.File.Content;

                    //    var newFile = context.Files.Add(file);

                    //    var fileAttributes = new List<FileAttribute>();

                    //    if (model.File.Attributes?.Length > 0)
                    //    {
                    //        for (int i = 0; i < model.File.Attributes.Length; i++)
                    //        {
                    //            fileAttributes.Add(new FileAttribute(model.File.Attributes[i]) { FileId = newFile.Id });
                    //        }
                    //    }

                    //    var newAttributes = context.FileAttributes.AddRange(fileAttributes);

                    //    var signedFile = new SignedFile();
                    //    signedFile.Guid = model.Guid;
                    //    signedFile.Key = model.Key;
                    //    signedFile.SessionId = model.SessionId;
                    //    signedFile.FileId = newFile.Id;

                    //    context.SignedFiles.Add(signedFile);
                    //    var result = await context.SaveChangesAsync();
                    //    transaction.Commit();
                    //}
                }
            }
        }
    }
}
