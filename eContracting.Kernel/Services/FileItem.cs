// <copyright file="ItemNotFoundPipeline.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Services
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Implementation of file item model.
    /// </summary>
    public class FileItem
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// Value of ZCCH_ST_FILE.FILEINDX
        /// </value>
        [JsonProperty("id")]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets title.
        /// </summary>
        /// <value>
        /// Value of ZCCH_ST_FILE.FILENAME or "Link_label" from ZCCH_ST_FILE.ATTRIB[].ATTRID
        /// </value>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets url.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets label.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
