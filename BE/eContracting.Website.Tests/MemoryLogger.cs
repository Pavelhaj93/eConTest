using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Website.Tests
{
    class MemoryLogger : ILogger
    {
        public readonly List<string> Data = new List<string>();

        public void Debug(string guid, string message)
        {
            this.Data.Add(message);
        }

        public void Error(string guid, string message)
        {
            this.Data.Add(message);
        }

        public void Error(string guid, Exception exception)
        {
            this.Data.Add(exception.Message);
        }

        public void Error(string guid, string message, Exception exception)
        {
            this.Data.Add(message);
        }

        public void Fatal(string guid, string message)
        {
            this.Data.Add(message);
        }

        public void Fatal(string guid, Exception exception)
        {
            this.Data.Add(exception.Message);
        }

        public void Fatal(string guid, string message, Exception exception)
        {
            this.Data.Add(message);
        }

        public void Info(string guid, string message)
        {
            this.Data.Add(message);
        }

        public void Warn(string guid, string message)
        {
            this.Data.Add(message);
        }
    }
}
