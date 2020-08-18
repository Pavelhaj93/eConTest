// <copyright file="XmlText.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Xmltext model.
    /// </summary>
    [Serializable]
    public class XmlText
    {
        /// <summary>
        /// Gets or sets index.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
    }
}
