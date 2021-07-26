using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class StepsViewModel
    {
        public IProcessStepModel[] Steps { get; }

        public StepsViewModel(IEnumerable<IProcessStepModel> steps)
        {
            this.Steps = steps.ToArray();
        }
    }
}
