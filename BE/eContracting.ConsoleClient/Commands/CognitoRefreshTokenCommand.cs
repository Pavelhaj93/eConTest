using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;
using eContracting.Models;
using eContracting.Services;
using Moq;

namespace eContracting.ConsoleClient.Commands
{
    public class CognitoRefreshTokenCommand : BaseCommand
    {
        protected readonly IRespApiService ApiService;
        protected readonly ILogger Logger;

        public CognitoRefreshTokenCommand(IRespApiService respApiService, ILogger logger, IConsole console) : base("refresh-token", console)
        {
            this.ApiService = respApiService;
            this.Logger = logger;
        }

        [Execute]
        public void Run([Argument(Alias = "token")] string refreshToken)
        {
            var settings = new MemorySettingsReaderService(new GlobalConfiguration());

            var service = new CognitoAuthService(settings, null, this.ApiService, this.Logger);
            var tokens = service.GetRefreshedTokens(new OAuthTokensModel("", "", refreshToken, ""));

            if (tokens == null)
            {
                this.Console.WriteLineError("No tokens received");
                return;
            }

            this.Console.WriteLineSuccess("New refresh token: ");
            this.Console.WriteLine("-----------");
            this.Console.WriteLine(tokens.RefreshToken);
            this.Console.WriteLine("-----------");
        }
    }
}
