using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eContracting.Models;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class StepsViewModel
    {
        public IStepModel[] Steps { get; }

        public StepsViewModel(IEnumerable<IStepModel> steps)
        {
            this.Steps = steps.ToArray();
        }

        public StepsViewModel(IStepsModel steps)
        {
            this.Steps = steps?.Steps.ToArray();
        }
    }
}
