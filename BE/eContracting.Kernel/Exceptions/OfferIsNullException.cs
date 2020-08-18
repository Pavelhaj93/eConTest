// <copyright file="DateOfBirthWrongFormatException.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Exceptions
{
    using System;

    /// <summary>
    /// Implenetaiotn of offer is null exception.
    /// </summary>
    public class OfferIsNullException : Exception
    {
        /// <summary>
        /// Creates new instance of <see cref="OfferIsNullException"/>
        /// </summary>
        public OfferIsNullException()
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="OfferIsNullException"/>
        /// </summary>
        /// <param name="message"></param>
        public OfferIsNullException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="OfferIsNullException"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public OfferIsNullException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
