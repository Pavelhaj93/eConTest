using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    /// <summary>
    /// Represents abstration layer for logger.
    /// </summary>
    public interface ILogger
    {
        void Info(string guid, string message);

        void Debug(string guid, string message);

        void Warn(string guid, string message);

        void Error(string guid, string message);

        void Error(string guid, Exception exception);

        void Error(string guid, string message, Exception exception);

        void Fatal(string guid, string message);

        void Fatal(string guid, Exception exception);

        void Fatal(string guid, string message, Exception exception);
    }
}
