using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    /// <summary>
    /// Join file(s) into group and optimize it into PDF file.
    /// </summary>
    public interface IFileOptimizer
    {
        /// <summary>
        /// Adds a file with <paramref name="name" /> to a group with <paramref name="groupKey" />.
        /// </summary>
        /// <remarks>
        ///     <para>If <paramref name="group"/> doesn't exist, creates new one.</para>
        ///     <para>
        ///         Parameter <paramref name="fileId"/> is used to find new file in <see cref="OptimizedFileGroupModel.Files"/> collection.
        ///         If any file with the same <paramref name="fileId"/> already exists in the group, throws <see cref="ApplicationException"/>.
        ///     </para>
        /// </remarks>
        /// <param name="group">Existing group definition from cache. Could be null.</param>
        /// <param name="groupKey">The group unique key.</param>
        /// <param name="fileId">Unique file key.</param>
        /// <param name="name">The file name.</param>
        /// <param name="content">The content content.</param>
        /// <param name="sessionId">Current session id.</param>
        /// <param name="guid">Offer unique identifier.</param>
        /// <returns>Current state of the group after adding new file.</returns>
        /// <exception cref="ApplicationException">Adding failed.</exception>
        Task<UploadGroupFileOperationResultModel> AddAsync(DbUploadGroupFileModel group, string groupKey, string fileId, string name, byte[] content, string sessionId, string guid);

        /// <summary>
        /// Removes specific file with <paramref name="fileId"/> from <paramref name="group"/>.
        /// </summary>
        /// <param name="group">Existing group.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>Updated group or null if group is empty.</returns>
        Task<DbUploadGroupFileModel> RemoveFileAsync(DbUploadGroupFileModel group, string fileId);

        /// <summary>
        /// Checks if the last added file does not cause exceeding the resulting total uploaded files size limit
        /// </summary>
        /// <param name="allGroups"></param>
        /// <param name="groupLastAdded"></param>
        /// <param name="fileIdLastAdded"></param>
        Task<UploadGroupFileOperationResultModel> EnforceOfferTotalFilesSizeAsync(List<DbUploadGroupFileModel> allGroups, DbUploadGroupFileModel groupLastAdded, string fileIdLastAdded);
    }
}
