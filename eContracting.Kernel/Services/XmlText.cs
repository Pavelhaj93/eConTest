// <copyright file="XmlText.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Services
{
    using System;

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
        /// Gets or sets text.
        /// </summary>
        public string Text { get; set; }
    }
}
