using System;

namespace eContracting.RweClient
{
    public class OfferIsNullException : Exception
    {
        public OfferIsNullException()
        {
        }

        public OfferIsNullException(string message)
            : base(message)
        {
        }

        public OfferIsNullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

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
