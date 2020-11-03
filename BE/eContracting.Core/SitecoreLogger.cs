using System;
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
        public void Info(string message)
        {
            Log.Info(message, this);
        }

        public void Debug(string message)
        {
            Log.Info(message, this);
        }

        public void Warn(string message)
        {
            Log.Warn(message, this);
        }

        public void Error(string message)
        {
            Log.Error(message, this);
        }

        public void Error(Exception exception)
        {
            Log.Error(exception.Message, this);
        }

        public void Error(string message, Exception exception)
        {
            Log.Error(message, exception, this);
        }

        public void Fatal(string message)
        {
            Log.Fatal(message, this);
        }

        public void Fatal(Exception exception)
        {
            Log.Fatal(exception.Message, this);
        }

        public void Fatal(string message, Exception exception)
        {
            Log.Fatal(message, exception, this);
        }
    }
}
