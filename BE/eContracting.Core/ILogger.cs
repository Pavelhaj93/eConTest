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
        void Info(string message);

        void Debug(string message);

        void Warn(string message);

        void Error(string message);

        void Error(Exception exception);

        void Error(string message, Exception exception);

        void Fatal(string message);

        void Fatal(Exception exception);

        void Fatal(string message, Exception exception);
    }
}
