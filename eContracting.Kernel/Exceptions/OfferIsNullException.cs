using System;

namespace eContracting.Kernel.Exceptions
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
}
