﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using eContracting.Models;
using eContracting.Services;

namespace eContracting.Models
{
    /// <summary>
    /// Represent response from 'CACHE_GET'.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ResponseCacheGetModel
    {
        /// <summary>
        /// Raw response from SAP.
        /// </summary>
        public readonly ZCCH_CACHE_GETResponse Response;

        public OFFER_TYPES Type
        {
            get
            {
                return (OFFER_TYPES)Enum.Parse(typeof(OFFER_TYPES), this.Response.ES_HEADER.CCHTYPE, true);
            }
        }

        public string Guid
        {
            get
            {
                return this.Response.ES_HEADER.CCHKEY;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Response"/> contains files.
        /// </summary>
        public bool HasFiles
        {
            get
            {
                return this.Response.ET_FILES.Length > 0;
            }
        }
        
        public OfferAttachmentModel[] Attachments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseCacheGetModel"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <exception cref="ArgumentNullException">response</exception>
        public ResponseCacheGetModel(ZCCH_CACHE_GETResponse response)
        {
            this.Response = response ?? throw new ArgumentNullException(nameof(response));
        }
    }
}
