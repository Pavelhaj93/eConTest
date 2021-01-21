using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Tests
{
    public class MemoryLogger : ILogger
    {
        public List<KeyValuePair<string, string>> Fatals { get; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Infos { get; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Warns { get; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Errors { get; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Debugs { get; } = new List<KeyValuePair<string, string>>();

        public void Debug(string guid, string message)
        {
            this.Debugs.Add(new KeyValuePair<string, string>(guid, message));
        }

        public void Error(string guid, string message)
        {
            this.Errors.Add(new KeyValuePair<string, string>(guid, message));
        }

        public void Error(string guid, Exception exception)
        {
            this.Errors.Add(new KeyValuePair<string, string>(guid, exception.Message));
        }

        public void Error(string guid, string message, Exception exception)
        {
            this.Errors.Add(new KeyValuePair<string, string>(guid, message));
        }

        public void Fatal(string guid, string message)
        {
            this.Fatals.Add(new KeyValuePair<string, string>(guid, message));
        }

        public void Fatal(string guid, Exception exception)
        {
            this.Fatals.Add(new KeyValuePair<string, string>(guid, exception.Message));
        }

        public void Fatal(string guid, string message, Exception exception)
        {
            this.Fatals.Add(new KeyValuePair<string, string>(guid, message));
        }

        public void Info(string guid, string message)
        {
            this.Infos.Add(new KeyValuePair<string, string>(guid, message));
        }

        public void Warn(string guid, string message)
        {
            this.Warns.Add(new KeyValuePair<string, string>(guid, message));
        }
    }
}
