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
                this.Configuration.ServiceUrl = "https://wti.rwe-services.cz:51012/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                this.Configuration.ServiceSignUrl = "https://wti.rwe-services.cz:51012/sap/bc/srt/xip/sap/zcrm_sign_stamp_merge/100/crm_sign_stamp_merge/crm_sign_stamp_merge";
            }
            else if (environment == "dev")
            {
                this.Configuration.ServiceUrl = "https://wti.rwe-services.cz:51018/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                this.Configuration.ServiceSignUrl = "https://wti.rwe-services.cz:51026/sap/bc/srt/xip/sap/zcrm_sign_stamp_merge/100/crm_sign_stamp_merge/crm_sign_stamp_merge";
            }
            else if (environment == "prod")
            {
                this.Configuration.ServiceUrl = "https://wpi.rwe-services.cz:41007/sap/bc/srt/rfc/sap/zcch_cache_api/100/zcch_cache_api/zcch_cache_api";
                this.Configuration.ServiceSignUrl = "https://wti.rwe-services.cz:41007/sap/bc/srt/xip/sap/zcrm_sign_stamp_merge/100/crm_sign_stamp_merge/crm_sign_stamp_merge";
            }
            else
            {
                throw new InvalidOperationException($"Unknown environment: {environment}");
            }
        }
    }
}
