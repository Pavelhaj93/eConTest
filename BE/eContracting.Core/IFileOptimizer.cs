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
        /// <remarks>If grou with <paramref name="groupKey"/> doesn't exist, creates new one.</remarks>
        /// <param name="groupKey">The group unique key.</param>
        /// <param name="name">The file name.</param>
        /// <param name="content">The content content.</param>
        /// <returns>Current state of the group after adding new file.</returns>
        /// <exception cref="ApplicationException">Adding failed.</exception>
        Task<OptimizedFileGroupModel> AddAsync(string groupKey, string name, byte[] content);

        /// <summary>
        /// Gets current group state by <paramref name="groupKey"/>.
        /// </summary>
        /// <param name="groupKey">The group unique key.</param>
        /// <returns>Current state of the group or null if group doens't exist (or group is empty).</returns>
        Task<OptimizedFileGroupModel> GetAsync(string groupKey);

        /// <summary>
        /// Removes specific file with <paramref name="fileId"/> from group by <paramref name="groupKey"/>.
        /// </summary>
        /// <param name="groupKey">The group unique key.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>True if file was removed, otherwise false.</returns>
        Task<bool> RemoveAsync(string groupKey, string fileId);
    }
}
