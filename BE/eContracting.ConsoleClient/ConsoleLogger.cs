using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop.Abstractions;

namespace eContracting.ConsoleClient
{
    public class ConsoleLogger : ILogger
    {
        protected readonly IConsole Console;

        public bool Suspended { get; set; }

        public ConsoleLogger(IConsole console)
        {
            this.Console = console;
        }

        public void Debug(string guid, string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLine(message);
            }
        }

        public void Error(string guid, string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message);
            }
        }

        public void Error(string guid, Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(exception.ToString());
            }
        }

        public void Error(string guid, string message, Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message + Environment.NewLine + exception.ToString());
            }
        }

        public void Fatal(string guid, string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message);
            }
        }

        public void Fatal(string guid, Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(exception.ToString());
            }
        }

        public void Fatal(string guid, string message, Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message + Environment.NewLine + exception.ToString());
            }
        }

        public void Info(string guid, string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLine(message);
            }
        }

        public void Warn(string guid, string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLine(message);
            }
        }
    }

    public static class ConsoleLoggerExtensions
    {
        public static void Suspend(this ILogger logger, bool suspend)
        {
            (logger as ConsoleLogger).Suspended = suspend;
        }
    }
}
