using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.ConsoleClient
{
    class ConsoleLoggerSuspender : IDisposable
    {
        private ILogger Logger;

        public ConsoleLoggerSuspender(ILogger logger) : this(logger, true)
        {
        }

        public ConsoleLoggerSuspender(ILogger logger, bool suspend)
        {
            this.Logger = logger;
            this.Logger.Suspend(suspend);
        }

        public void Dispose()
        {
            this.Logger.Suspend(false);
        }
    }
}
