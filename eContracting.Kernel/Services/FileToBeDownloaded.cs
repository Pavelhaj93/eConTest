// <copyright file="FileToBeDownloaded.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

using System;
using System.Collections.Generic;

namespace eContracting.Kernel.Services
{
    /// <summary>
    /// Implementation of file to be donloaded model.
    /// </summary>
    [Serializable]
    public class FileToBeDownloaded
    {
        /// <summary>
        /// Gets or set index.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets file number.
        /// </summary>
        public string FileNumber { get; set; }

        /// <summary>
        /// Gets or sets a file type.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets file name.
        /// </summary>
        public string FileName { get; set; }

        ///Gets or sets a value indicating whether signing is required.
        public string SignRequired { get; set; }

        /// <summary>
        /// Gets or sets file content.
        /// </summary>
        public List<Byte> FileContent { get; set; }
    }

}
