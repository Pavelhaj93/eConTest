using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    /// <summary>
    /// Original file info in optimized group.
    /// </summary>
    public class FileInOptimizedGroupModel
    {
        /// <summary>
        /// The original file name.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// The unique file key.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The MIME type.
        /// </summary>
        public readonly string MimeType;

        /// <summary>
        /// The original file size.
        /// </summary>
        public readonly int OriginalSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInOptimizedGroupModel"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="key">The key.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="originalSize">The original size.</param>
        /// <exception cref="ArgumentException">
        /// File name is empty - fileName
        /// or
        /// File key is empty - key
        /// or
        /// Mime type is empty - mimeType
        /// </exception>
        public FileInOptimizedGroupModel(string fileName, string key, string mimeType, int originalSize)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is empty", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("File key is empty", nameof(key));
            }

            if (string.IsNullOrWhiteSpace(mimeType))
            {
                throw new ArgumentException("Mime type is empty", nameof(mimeType));
            }

            this.FileName = fileName;
            this.Key = key;
            this.MimeType = mimeType;
            this.OriginalSize = originalSize;
        }
    }
}
