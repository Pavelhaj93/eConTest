using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public interface IWorkingDay : IBaseSitecoreModel
    {
        int DayInWeek { get; set; }

        string WorkingHoursFrom { get; set; }

        string WorkingHoursTo { get; set; }

        string OperatorHoursFrom { get; set; }

        string OperatorHoursTo { get; set; }
    }
}
