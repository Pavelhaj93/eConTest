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
        public readonly IProcessModel[] Processes;
        public readonly IProcessTypeModel[] ProcessTypes;

        public ProcessTypesSwitcherViewModel(string process, string processType, NameValueCollection query, IProcessModel[] processes, IProcessTypeModel[] processTypes)
        {
            this.Process = process;
            this.ProcessType = processType;
            this.Query = query;
            this.Processes = processes;
            this.ProcessTypes = processTypes;
        }

        public ProcessTypesSwitcherViewModel()
        {
            var processCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS];
            var processTypeCode = HttpContext.Current.Request.QueryString[Constants.QueryKeys.PROCESS_TYPE];
            var settingsReader = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsReaderService>();

            this.Process = processCode;
            this.ProcessType = processTypeCode;
            this.Processes = settingsReader.GetAllProcesses().ToArray();
            this.ProcessTypes = settingsReader.GetAllProcessTypes().ToArray();
            this.Query = HttpContext.Current.Request.QueryString;
        }
    }
}
