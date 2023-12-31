﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;

namespace eContracting
{
    /// <summary>
    /// Wrapper over Sitecore logger.
    /// </summary>
    /// <seealso cref="eContracting.ILogger" />
    [ExcludeFromCodeCoverage]
    public class SitecoreLogger : ILogger
    {
        public SitecoreLogger()
        {
        }

        /// <inheritdoc/>
        public void Info(string guid, string message)
        {
            Log.Info($"[{guid}] {message}", this);
        }

        /// <inheritdoc/>
        public void Debug(string guid, string message)
        {
            Log.Debug($"[{guid}] {message}", this);
        }

        /// <inheritdoc/>
        public void Warn(string guid, string message)
        {
            Log.Warn($"[{guid}] {message}", this);
        }

        /// <inheritdoc/>
        public void Error(string guid, string message)
        {
            Log.Error($"[{guid}] {message}", this);
        }

        /// <inheritdoc/>
        public void Error(string guid, Exception exception)
        {
            Log.Error($"[{guid}] {exception.Message}", exception, this);
        }

        /// <inheritdoc/>
        public void Error(string guid, string message, Exception exception)
        {
            Log.Error($"[{guid}] {message}", exception, this);
        }

        /// <inheritdoc/>
        public void Fatal(string guid, string message)
        {
            Log.Fatal($"[{guid}] {message}", this);
        }

        /// <inheritdoc/>
        public void Fatal(string guid, Exception exception)
        {
            Log.Fatal($"[{guid}] {exception.Message}", exception, this);
        }

        /// <inheritdoc/>
        public void Fatal(string guid, string message, Exception exception)
        {
            Log.Fatal($"[{guid}] {message}", exception, this);
        }
    }
}
