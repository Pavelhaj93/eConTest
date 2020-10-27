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

        public void Debug(string message)
        {
            this.Data.Add(message);
        }

        public void Error(string message)
        {
            this.Data.Add(message);
        }

        public void Error(Exception exception)
        {
            this.Data.Add(exception.Message);
        }

        public void Error(string message, Exception exception)
        {
            this.Data.Add(message);
        }

        public void Fatal(string message)
        {
            this.Data.Add(message);
        }

        public void Fatal(Exception exception)
        {
            this.Data.Add(exception.Message);
        }

        public void Fatal(string message, Exception exception)
        {
            this.Data.Add(message);
        }

        public void Info(string message)
        {
            this.Data.Add(message);
        }

        public void Warn(string message)
        {
            this.Data.Add(message);
        }
    }
}
