// <copyright file="AuthenticationDataItem.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Utils
{
    using System;
    using System.Globalization;

    [Serializable]
    public class AuthenticationDataItem
    {
        /// <summary>
        /// Gets or sets item type.
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets Campaign value.
        /// </summary>
        public string Campaign { get; set; }

        /// <summary>
        /// Gets or sets commodity value.
        /// </summary>
        public string Commodity { get; set; }

        /// <summary>
        /// Gets or sets creation timestamp.
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// If offer type is INDI.
        /// </summary>
        public bool IsIndi { get; set; }

        /// <summary>
        /// Gets or sets item value.
        /// </summary>
        public string ItemValue { get; set; }

        /// <summary>
        /// Gets or sets item friendly name.
        /// </summary>
        public string ItemFriendlyName { get; set; }

        /// <summary>
        /// Gets or sets date of birth.
        /// </summary>
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets expiration date.
        /// </summary>
        public string ExpDate { get; set; }

        /// <summary>
        /// Gets or sets if offer is accpeted.
        /// </summary>
        public bool IsAccepted { get; set; }

        public OfferTypes OfferType
        {
            get
            {
                if (this.IsRetention)
                {
                    return OfferTypes.Retention;
                }
                else if (this.IsAcquisition)
                {
                    return OfferTypes.Acquisition;
                }
                else
                {
                    return OfferTypes.Default;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether offer is retention type.
        /// </summary>
        public bool IsRetention { protected get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is acquisition.
        /// </summary>
        public bool IsAcquisition { protected get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether offer contains offers (document is without AD1)
        /// </summary>
        public bool HasVoucher { get; set; }

        /// <summary>
        /// Gets or sets expiration flag for offer.
        /// </summary>
        public bool OfferIsExpired { get; set; }

        /// <summary>
        /// Gets converfted expiration date.
        /// </summary>
        public DateTime ExpDateConverted
        {
            get
            {
                DateTime outputDateTimeValue;
                DateTime.TryParseExact(this.ExpDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outputDateTimeValue);
                return outputDateTimeValue;
            }
        }

        /// <summary>
        /// Gets formatted expiration date.
        /// </summary>
        public string ExpDateFormatted
        {
            get
            {
                return this.ExpDateConverted.ToString("dd.MM.yyy");
            }
        }
    }
}
