using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;
using eContracting.Models;
using eContracting.Storage;
using MongoDB.Driver.Linq;

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

            await this.RemoveSignedFileAsync(search);
            await this.RemoveGroupAsync(search);
        }

        /// <inheritdoc/>
        public async Task<DbUploadGroupFileModel> FindGroupAsync(DbSearchParameters search)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var query = context.UploadGroups.AsQueryable();

                if (!string.IsNullOrEmpty(search.Key))
                {
                    query = query.Where(x => x.Key == search.Key);
                }

                if (!string.IsNullOrEmpty(search.Guid))
                {
                    query = query.Where(x => x.Guid == search.Guid);
                }

                if (!string.IsNullOrEmpty(search.SessionId))
                {
                    query = query.Where(x => x.SessionId == search.SessionId);
                }

                var result = await query.FirstOrDefaultAsync();

                if (result != null)
                {
                    var group = result.ToModel();

                    if (result.OutputFileId > 0)
                    {
                        var file = await this.FindFileAsync(context, result.OutputFileId);

                        if (file != null)
                        {
                            group.OutputFile = file;
                        }
                    }

                    var origFiles = context.UploadGroupOriginalFiles.Where(x => x.GroupId == result.Id);

                    if (origFiles.Any())
                    {
                        var files = await this.FindFilesAsync(context, origFiles.Select(x => x.FileId));

                        foreach (var file in files)
                        {
                            file.Key = origFiles.First(x => x.FileId == file.Id).FileKey;
                        }

                        group.OriginalFiles.AddRange(files);
                    }

                    return group;
                }

                return null;
            }
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

                if (result != null)
                {
                    var signedFile = result.ToModel();
                    
                    if (result.FileId > 0)
                    {
                        var file = await this.FindFileAsync(context, result.FileId);
                        signedFile.File = file;
                    }

                    return signedFile;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task RemoveGroupAsync(DbSearchParameters search)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var query = context.UploadGroups.AsQueryable();

                if (!string.IsNullOrEmpty(search.Key))
                {
                    query = query.Where(x => x.Key == search.Key);
                }

                if (!string.IsNullOrEmpty(search.Guid))
                {
                    query = query.Where(x => x.Guid == search.Guid);
                }

                if (!string.IsNullOrEmpty(search.SessionId))
                {
                    query = query.Where(x => x.SessionId == search.SessionId);
                }

                var records = query.ToArray();

                if (records.Length > 0)
                {
                    await this.RemoveAsync(context, records);
                }
            }
        }

        /// <inheritdoc/>
        public async Task RemoveSignedFileAsync(DbSearchParameters search)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var query = context.SignedFiles.AsQueryable();

                if (!string.IsNullOrEmpty(search.Key))
                {
                    query = query.Where(x => x.Key == search.Key);
                }

                if (!string.IsNullOrEmpty(search.Guid))
                {
                    query = query.Where(x => x.Guid == search.Guid);
                }

                if (!string.IsNullOrEmpty(search.SessionId))
                {
                    query = query.Where(x => x.SessionId == search.SessionId);
                }

                var records = query.ToArray();

                if (records.Length > 0)
                {
                    await this.RemoveAsync(context, records);
                }
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync(DbUploadGroupFileModel groupModel)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var existingDbModel = await context.UploadGroups.FirstOrDefaultAsync(x => x.Id == groupModel.Id);

                if (existingDbModel != null)
                {
                    if (groupModel.OriginalFiles.Count == 0)
                    {
                        await this.RemoveAsync(context, new[] { existingDbModel });
                        return;
                    }

                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var file = await context.Files.FirstOrDefaultAsync(x => x.Id == existingDbModel.OutputFileId);
                        // we need to update only content of outputfile
                        file.Content = groupModel.OutputFile.Content;
                        var entry = context.Entry(file);
                        entry.State = EntityState.Modified;
                        await context.SaveChangesAsync();

                        var allDbOriginalFiles = await context.UploadGroupOriginalFiles.Where(x => x.GroupId == existingDbModel.Id).ToListAsync();
                        var allDbOriginalFileIds = allDbOriginalFiles.Select(x => x.FileId).ToList();

                        var currentFileIds = groupModel.OriginalFiles.Select(x => x.Id).ToArray();
                        var removeFileIds = allDbOriginalFileIds.Where(x => !currentFileIds.Contains(x)).ToArray();
                        var newFiles = groupModel.OriginalFiles.Where(x => x.Id == 0).ToArray();

                        if (removeFileIds.Length > 0)
                        {
                            var removeFiles = context.Files.Where(x => removeFileIds.Contains(x.Id)).ToArray();
                            await this.RemoveAsync(context, transaction, removeFiles);
                        }

                        for (int i = 0; i < newFiles.Length; i++)
                        {
                            var newFile = newFiles[i];
                            await this.SaveAsync(context, transaction, newFile);

                            var rel = new UploadGroupOriginalFile();
                            rel.FileId = newFile.Id;
                            rel.GroupId = groupModel.Id;
                            rel.FileKey = newFile.Key;
                            context.UploadGroupOriginalFiles.Add(rel);
                            await context.SaveChangesAsync();
                        }

                        // it's not necessary to do update of the files because they don't change during lifecycle

                        transaction.Commit();
                    }
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var file = new File(groupModel.OutputFile);
                        var fileAttr = groupModel.OutputFile.Attributes?.Select(x => new FileAttribute(x));
                        await this.SaveAsync(context, transaction, file, fileAttr);

                        var group = new UploadGroup(groupModel);
                        group.OutputFileId = file.Id;
                        context.UploadGroups.Add(group);
                        await context.SaveChangesAsync();
                        group.Model.Id = group.Id;

                        foreach (var item in groupModel.OriginalFiles)
                        {
                            await this.SaveAsync(context, transaction, item);

                            var rel = new UploadGroupOriginalFile();
                            rel.FileId = item.Id;
                            rel.GroupId = group.Id;
                            rel.FileKey = item.Key;
                            context.UploadGroupOriginalFiles.Add(rel);
                            await context.SaveChangesAsync();
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task SetAsync(DbSignedFileModel model)
        {
            using (var context = new DatabaseContext(this.ConnectionString))
            {
                var dbModel = await context.SignedFiles.FirstOrDefaultAsync(x => x.Key == model.Key && x.Guid == model.Guid && x.SessionId == model.SessionId);

                if (dbModel != null)
                {
                    if (dbModel.FileId > 0)
                    {
                        var file = await this.FindFileAsync(context, dbModel.FileId);
                        file.Content = model.File.Content;
                        var entry = context.Entry(file);
                        entry.State = EntityState.Modified;
                        var result = await context.SaveChangesAsync();
                    }
                    else
                    {
                        var file = new File(model.File);
                        context.Files.Add(file);
                        var result = await context.SaveChangesAsync();

                        if (model.File.Attributes.Count > 0)
                        {
                            context.FileAttributes.AddRange(model.File.Attributes.Select(x => new FileAttribute(x) { FileId = file.Id }));
                            var result2 = await context.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var file = new File(model.File);
                        context.Files.Add(file);
                        await context.SaveChangesAsync();

                        if (model.File.Attributes.Count > 0)
                        {
                            var fileAttrs = model.File.Attributes.Select(x => new FileAttribute(x) { FileId = file.Id });
                            context.FileAttributes.AddRange(fileAttrs);
                            await context.SaveChangesAsync();
                        }

                        var signedFile = new SignedFile(model);
                        signedFile.FileId = file.Id;
                        context.SignedFiles.Add(signedFile);
                        await context.SaveChangesAsync();

                        transaction.Commit();
                    }
                }
            }
        }

        protected async Task<DbFileModel> FindFileAsync(DatabaseContext context, int fileId)
        {
            var result = await this.FindFilesAsync(context, new[] { fileId });
            return result.FirstOrDefault();
        }

        protected async Task<IEnumerable<DbFileModel>> FindFilesAsync(DatabaseContext context, IEnumerable<int> ids)
        {
            var list = new List<DbFileModel>();

            var files = context.Files.Where(x => ids.Contains(x.Id)).ToArray();

            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    var attributes = context.FileAttributes.Where(x => x.FileId == file.Id).ToArray();
                    list.Add(file.ToModel(attributes));
                }
            }

            return list;
        }

        protected async Task<(File, IEnumerable<FileAttribute>)> SaveAsync(DatabaseContext context, DbContextTransaction transaction, DbFileModel file)
        {
            var dbFile = new File(file);
            var attributes = file.Attributes.Select(x => new FileAttribute(x)).ToArray();
            await this.SaveAsync(context, transaction, dbFile, attributes);
            file.Id = dbFile.Id;
            attributes.ForEach(x => { x.Model.Id = x.Id; });
            return (dbFile, attributes);
        }

        /// <summary>
        /// Save <paramref name="file"/> with its <paramref name="attributes"/> in transaction.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file.</param>
        /// <param name="attributes">The attributes.</param>
        protected async Task SaveAsync(DatabaseContext context, File file, IEnumerable<FileAttribute> attributes)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                await this.SaveAsync(context, transaction, file, attributes);
                transaction.Commit();
            }
        }

        protected async Task SaveAsync(DatabaseContext context, DbContextTransaction transaction, File file, IEnumerable<FileAttribute> attributes)
        {
            if (file.Id > 1)
            {
                var entry = context.Entry(file);
                entry.State = EntityState.Modified;
                await context.SaveChangesAsync();

                foreach (var attr in attributes)
                {
                    attr.FileId = file.Id;

                    if (attr.Id > 0)
                    {
                        var entry2 = context.Entry(attr);
                        entry2.State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        context.FileAttributes.Add(attr);
                        await context.SaveChangesAsync();
                    }
                }

                await context.SaveChangesAsync();
            }
            else
            {
                context.Files.Add(file);
                await context.SaveChangesAsync();

                if (attributes.Any())
                {
                    attributes.ForEach(x => { x.FileId = file.Id; });
                    context.FileAttributes.AddRange(attributes);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected async Task RemoveAsync(DatabaseContext context, IEnumerable<File> files)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                await this.RemoveAsync(context, transaction, files);
                transaction.Commit();
            }
        }

        protected async Task RemoveAsync(DatabaseContext context, DbContextTransaction transaction, IEnumerable<File> files)
        {
            foreach (var file in files)
            {
                var attr = context.FileAttributes.Where(x => x.FileId == file.Id).ToArray();

                if (attr.Any())
                {
                    context.FileAttributes.RemoveRange(attr);
                    await context.SaveChangesAsync();
                }

                var rel = context.UploadGroupOriginalFiles.Where(x => x.FileId == file.Id).ToArray();

                if (rel.Any())
                {
                    context.UploadGroupOriginalFiles.RemoveRange(rel);
                    await context.SaveChangesAsync();
                }

                context.Files.Remove(file);
                await context.SaveChangesAsync();
            }
        }

        protected async Task RemoveAsync(DatabaseContext context, IEnumerable<UploadGroup> groups)
        {
            foreach (var group in groups)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var allFiles = new List<File>();

                    if (group.OutputFileId > 0)
                    {
                        var outputFile = await context.Files.Where(x => x.Id == group.OutputFileId).FirstOrDefaultAsync();

                        if (outputFile != null)
                        {
                            allFiles.Add(outputFile);
                        }
                    }

                    var rel = await context.UploadGroupOriginalFiles.Where(x => x.GroupId == group.Id).ToArrayAsync();

                    if (rel.Length > 0)
                    {
                        var relFiles = rel.Select(x => x.FileId).ToArray();
                        var files = await context.Files.Where(x => relFiles.Contains(x.Id)).ToArrayAsync();

                        if (files.Length > 0)
                        {
                            allFiles.AddRange(files);
                        }
                    }

                    context.UploadGroups.Remove(group);
                    await context.SaveChangesAsync();
                    await this.RemoveAsync(context, transaction, allFiles);
                    transaction.Commit();
                }
            }
        }

        protected async Task RemoveAsync(DatabaseContext context, IEnumerable<SignedFile> signedFiles)
        {
            foreach (var signedFile in signedFiles)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    File file = null;

                    if (signedFile.FileId > 0)
                    {
                        file = await context.Files.FirstOrDefaultAsync(x => x.Id == signedFile.FileId);
                    }

                    context.SignedFiles.Remove(signedFile);
                    await context.SaveChangesAsync();

                    if (file != null)
                    {
                        await this.RemoveAsync(context, transaction, new[] { file });
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
