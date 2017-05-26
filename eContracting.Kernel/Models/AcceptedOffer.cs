// <copyright file="AcceptedOffer.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Implememtaiton of Accpetd offer model.
    /// </summary>
    public class AcceptedOffer
    {
        public long _id{get;set;}
        /// <summary>
        /// Customer Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// If true The accepted offer has been sent to service
        /// </summary>
        public bool  SentToService { get; set; }
    }
}

