using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class StepsViewModel
    {
        public ProcessStepModel[] Steps { get; }

        public StepsViewModel(IEnumerable<ProcessStepModel> steps)
        {
            this.Steps = steps.ToArray();
        }
    }
}
