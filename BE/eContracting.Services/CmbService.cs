using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Services
{
    public class CmbService
    {
        public CallMeBackModel GetCmb(IDefinitionCombinationDataModel matrix)
        {
            var model = new CallMeBackModel();
            return model;
        }

        public IEnumerable<KeyValuePair<string, string>> GetAvailableTimes(ICallMeBackModalWindow datasource)
        {
            IWorkingDay workingDay = this.GetWorkingDay(datasource);
            throw new NotImplementedException();
        }

        protected internal IWorkingDay GetWorkingDay(ICallMeBackModalWindow datasource)
        {
            var currentDay = (DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
            IWorkingDay workingDay = this.GetCurrentWorkingDay(currentDay, datasource);

            if (workingDay != null)
            {
                TimeSpan from;

                if (!TimeSpan.TryParse(workingDay.WorkingHoursFrom, out from))
                {
                    from = new TimeSpan(9, 0, 0);
                }

                TimeSpan to;

                if (!TimeSpan.TryParse(workingDay.WorkingHoursTo, out to))
                {
                    to = new TimeSpan(17, 0, 0);
                }

                if (DateTime.Now.TimeOfDay > to)
                {
                    workingDay = this.GetCurrentWorkingDay(currentDay == 7 ? 0 : currentDay + 1, datasource);
                }
            }

            if (workingDay == null)
            {
                workingDay = datasource.AvailabilityMonday;
            }

            return workingDay;
        }

        protected internal IWorkingDay GetCurrentWorkingDay(int currentDay, ICallMeBackModalWindow datasource)
        {
            if (currentDay == datasource.AvailabilityMonday.DayInWeek)
            {
                return datasource.AvailabilityMonday;
            }
            else if (currentDay == datasource.AvailabilityThuesday.DayInWeek)
            {
                return datasource.AvailabilityThuesday;
            }
            else if (currentDay == datasource.AvailabilityWednesday.DayInWeek)
            {
                return datasource.AvailabilityWednesday;
            }
            else if (currentDay == datasource.AvailabilityThursday.DayInWeek)
            {
                return datasource.AvailabilityThursday;
            }
            else if (currentDay == datasource.AvailabilityFriday.DayInWeek)
            {
                return datasource.AvailabilityFriday;
            }
            else
            {
                return null;
            }
        }
    }
}
