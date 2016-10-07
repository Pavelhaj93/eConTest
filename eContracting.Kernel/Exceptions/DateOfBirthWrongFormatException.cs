using System;

namespace eContracting.Kernel.Exceptions
{
    public class DateOfBirthWrongFormatException : Exception
    {
        public DateOfBirthWrongFormatException()
        {
        }

        public DateOfBirthWrongFormatException(string message)
            : base(message)
        {
        }

        public DateOfBirthWrongFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
