using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.ConsoleClient.OfferActions;
using eContracting.Services;
using Microsoft.Extensions.Options;

namespace eContracting.ConsoleClient
{
    class OfferCommand : BaseCommand
    {
        public override string Key { get; protected set; } = "offer";
        protected readonly ContextData Context;

        public OfferCommand(IEnumerable<IOfferAction> actions, IConsole console, ContextData contextData) : base(actions, console)
        {
            this.Context = contextData;
        }

        [Execute]
        public void Execute(string guid)
        {
            this.Context.Guid = guid;
        }
    }
}
