// <copyright file="AcceptOfferJob.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Exceptions
{
    using System;

    /// <summary>
    /// Implementation of wrong birth date exception.
    /// </summary>
    public class DateOfBirthWrongFormatException : Exception
    {
        /// <summary>
        /// Creates new instance of the <see cref="DateOfBirthWrongFormatException"/>.
        /// </summary>
        public DateOfBirthWrongFormatException()
        {
        }

        /// <summary>
        /// Creates new instance of the <see cref="DateOfBirthWrongFormatException"/>.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public DateOfBirthWrongFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates new instance of the <see cref="DateOfBirthWrongFormatException"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public DateOfBirthWrongFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
