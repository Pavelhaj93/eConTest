// <copyright file="AcceptOfferJob.cs" company="Actum">
// Copyright © 2016 Respective owners
// </copyright>

namespace eContracting.Kernel.Agents
{
    using eContracting.Kernel.Models;
    using eContracting.Kernel.Services;
    using MongoDB.Bson;
    using MongoDB.Driver.Builders;
    using Sitecore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of the accepted offer job.
    /// </summary>
    [UsedImplicitly]
    public class AcceptOfferJob
    {
        /// <summary>
        /// RUn method for the job.
        /// </summary>
        [UsedImplicitly]
        public void Run()
        {
            int failtime = 0;
            RweClient client = new RweClient();
            var offersNotsent = client.GetNotSentOffers();
            foreach (AcceptedOffer offer in offersNotsent.ToList())
            {

                if (client.AcceptOffer(offer.Guid))
                {
                    failtime++;
                }
                if (failtime > 3)
                {
                    break;
                }
            }
        }
    }
}
