using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient.OfferActions
{
    abstract class BaseOfferAction : BaseAction, IOfferAction
    {
        protected readonly IOptions<GlobalConfiguration> Options;
        protected readonly ContextData Context;

        protected BaseOfferAction(IConsole console, IOptions<GlobalConfiguration> options, ContextData contextData) : base(console)
        {
            this.Options = options;
            this.Context = contextData;
        }
    }
}
