using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Storage;

namespace eContracting
{
    /// <summary>
    /// Cache for user files relevant only for current user. Data cannot be shared among sessions.
    /// </summary>
    public interface IUserFileCacheService
    {
        /// <summary>
        /// Adds or rewrite <paramref name="group"/> under specific <see cref="UploadGroup.Key"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <exception cref="ApplicationException">When data were not stored.</exception>
        Task SetAsync(UploadGroup group);

        /// <summary>
        /// Adds or rewrite <paramref name="file"/> under specific <see cref="SignedFile.Key"/>.
        /// </summary>
        /// <param name="file">The signed file model.</param>
        /// <exception cref="ApplicationException">When data were not stored.</exception>
        Task SetAsync(SignedFile file);

        /// <summary>
        /// Gets the signed file by specific <paramref name="search"/> parameters.
        /// </summary>
        /// <param name="search">The search parameters.</param>
        /// <returns>File or null.</returns>
        Task<SignedFile> GetSignedFileAsync(DbSearchParameters search);

        /// <summary>
        /// Gets the group data by specific <paramref name="search"/> parameters.
        /// </summary>
        /// <param name="search">The search parameters.</param>
        /// <returns>Data or null.</returns>
        Task<UploadGroup> GetGroupAsync(DbSearchParameters search);

        /// <summary>
        /// Removes the signed file under specific <paramref name="search"/> parameters.
        /// </summary>
        /// <param name="search">The search parameters.</param>
        Task RemoveSignedFileAsync(DbSearchParameters search);

        /// <summary>
        /// Removes the group under specific <paramref name="search"/> parameters>.
        /// </summary>
        /// <param name="search">The search parameters.</param>
        Task RemoveGroupAsync(DbSearchParameters search);

        /// <summary>
        /// Removes all data base on <paramref name="search"/> parameters.
        /// </summary>
        /// <param name="search">The search parameters.</param>
        Task ClearAsync(DbSearchParameters search);
    }
}
