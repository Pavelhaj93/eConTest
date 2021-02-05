using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using eContracting.Models;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class ProcessTypesSwitcherViewModel
    {
        public readonly string Process;
        public readonly string ProcessType;
        public readonly NameValueCollection Query;
        public readonly ProcessModel[] Processes;
        public readonly ProcessTypeModel[] ProcessTypes;

        public ProcessTypesSwitcherViewModel(string process, string processType, NameValueCollection query, ProcessModel[] processes, ProcessTypeModel[] processTypes)
        {
            this.Process = process;
            this.ProcessType = processType;
            this.Query = query;
            this.Processes = processes;
            this.ProcessTypes = processTypes;
        }

        public ProcessTypesSwitcherViewModel()
        {
            var settingsReader = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();
            var cache = ServiceLocator.ServiceProvider.GetRequiredService<IUserDataCacheService>();
            var data = cache.Get<OfferCacheDataModel>(Constants.CacheKeys.OFFER_IDENTIFIER);

            this.Process = data.Process;
            this.ProcessType = data.ProcessType;
            this.Processes = settingsReader.GetAllProcesses().ToArray();
            this.ProcessTypes = settingsReader.GetAllProcessTypes().ToArray();
            this.Query = HttpContext.Current.Request.QueryString;
        }
    }
}
