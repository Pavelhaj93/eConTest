using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consinloop;
using Consinloop.Abstractions;
using Consinloop.Attributes;

namespace eContracting.ConsoleClient.Commands
{
    class SwitchServerCommand : BaseCommand
    {
        protected readonly GlobalConfiguration Configuration;

        public SwitchServerCommand(GlobalConfiguration configuration, IConsole console) : base("environment", console)
        {
            this.Configuration = configuration;
            this.AliasKey = "env";
            this.Description = "switch environment between 'dev' and 'test'";
        }

        [Execute]
        public void Execute(string environment)
        {
            if (environment == "test")
            {
                this.Configuration.ServiceUrl = "https://wd-wcc.rwe-services.cz:8110/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                this.Configuration.ServiceSignUrl = "https://wd-wcc.rwe-services.cz:8110/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            }
            else if (environment == "dev")
            {
                this.Configuration.ServiceUrl = "https://wd-wcc.rwe-services.cz:8109/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                this.Configuration.ServiceSignUrl = "https://wd-wcc.rwe-services.cz:8109/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
            }
            else
            {
                throw new InvalidOperationException($"Unknown environment: {environment}");
            }
        }
    }
}
