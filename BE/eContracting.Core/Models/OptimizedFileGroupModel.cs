using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Optimized group of <see cref="Files"/> with optimized (grouped) <see cref="File"/>.
    /// </summary>
    public class OptimizedFileGroupModel
    {
        /// <summary>
        /// The group identifier.
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// The group optimized size.
        /// </summary>
        public int Size
        {
            get
            {
                return this.File.Length;
            }
        }

        /// <summary>
        /// The optimized file (PDF expected) of all <see cref="Files"/> in the group.
        /// </summary>
        public readonly byte[] File;

        /// <summary>
        /// All related original files.
        /// </summary>
        public readonly IEnumerable<FileInOptimizedGroupModel> Files;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedFileGroupModel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="files">The files.</param>
        /// <param name="file">The file.</param>
        /// <exception cref="ArgumentException">
        /// Value cannot be null - id
        /// or
        /// No files defined - files
        /// or
        /// File content cannot be empty - file
        /// </exception>
        /// <exception cref="ArgumentNullException">file</exception>
        public OptimizedFileGroupModel(string id, IEnumerable<FileInOptimizedGroupModel> files, byte[] file)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Value cannot be null", nameof(id));
            }

            if (!files?.Any() ?? false)
            {
                throw new ArgumentException("No files defined", nameof(files));
            }

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Length == 0)
            {
                throw new ArgumentException("File content cannot be empty", nameof(file));
            }

            this.Id = id;
            this.Files = files;
            this.File = file;
        }
    }
}
