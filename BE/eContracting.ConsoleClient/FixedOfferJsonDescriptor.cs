using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using eContracting.Services;
using Glass.Mapper.Sc;

namespace eContracting.ConsoleClient
{
    class FixedOfferJsonDescriptor : OfferJsonDescriptor
    {
        public FixedOfferJsonDescriptor(
            ILogger logger,
            ITextService textService,
            ISitecoreContext context,
            IOfferService apiService,
            ISettingsReaderService settingsReaderService) : base(logger, textService, context, apiService, settingsReaderService)
        {
        }
    }
}
