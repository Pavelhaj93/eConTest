using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using eContracting.Models;

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
    }
}
