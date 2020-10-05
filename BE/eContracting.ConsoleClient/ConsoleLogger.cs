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

        public ConsoleLogger(IConsole console)
        {
            this.Console = console;
        }

        public void Debug(string message)
        {
            this.Console.WriteLine(message);
        }

        public void Error(string message)
        {
            this.Console.WriteLineError(message);
        }

        public void Error(Exception exception)
        {
            this.Console.WriteLineError(exception.ToString());
        }

        public void Error(string message, Exception exception)
        {
            this.Console.WriteLineError(message + Environment.NewLine + exception.ToString());
        }

        public void Fatal(string message)
        {
            this.Console.WriteLineError(message);
        }

        public void Fatal(Exception exception)
        {
            this.Console.WriteLineError(exception.ToString());
        }

        public void Fatal(string message, Exception exception)
        {
            this.Console.WriteLineError(message + Environment.NewLine + exception.ToString());
        }

        public void Info(string message)
        {
            this.Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            this.Console.WriteLine(message);
        }
    }
}
