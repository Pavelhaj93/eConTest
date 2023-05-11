using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    /// <summary>
    /// Local / offline sign service to simulate signature.
    /// </summary>
    /// <example>
    /// Patch file:
    /// <code>
    ///   <configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
    ///     <sitecore>
    ///       <services>
    ///         <register serviceType="eContracting.ISignService, eContracting.Core">
    ///           <patch:attribute name="implementationType">eContracting.Services.LocalFakeSignService, eContracting.Services</patch:attribute>
    ///         </register>
    ///       </services>
    ///     </sitecore>
    ///   </configuration>
    /// </code>
    /// </example>
    [ExcludeFromCodeCoverage]
    public class LocalFakeSignService : ISignService
    {
        protected readonly ILogger Logger;

        public LocalFakeSignService(ILogger logger)
        {
            this.Logger = logger;
        }

        public OfferAttachmentModel Sign(OfferModel offer, OfferAttachmentModel file, byte[] signature)
        {
            this.Logger.Info(offer.Guid, "Fake sign service called.");
            return file;
        }
    }
}
