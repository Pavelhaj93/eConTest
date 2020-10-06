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

        public void Debug(string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLine(message);
            }
        }

        public void Error(string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message);
            }
        }

        public void Error(Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(exception.ToString());
            }
        }

        public void Error(string message, Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message + Environment.NewLine + exception.ToString());
            }
        }

        public void Fatal(string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message);
            }
        }

        public void Fatal(Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(exception.ToString());
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLineError(message + Environment.NewLine + exception.ToString());
            }
        }

        public void Info(string message)
        {
            if (!this.Suspended)
            {
                this.Console.WriteLine(message);
            }
        }

        public void Warn(string message)
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
