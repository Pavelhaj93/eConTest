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
        /// Gets or sets title.
        /// </summary>
        [JsonProperty("title")]
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets url.
        /// </summary>
        [JsonProperty("url")]
        public String Url { get; set; }

        /// <summary>
        /// Gets or sets label.
        /// </summary>
        [JsonProperty("label")]
        public String Label { get; set; }
    }
}
