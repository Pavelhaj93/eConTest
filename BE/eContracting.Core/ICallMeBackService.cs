using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting
{
    public interface ICallMeBackService
    {
        IEnumerable<KeyValuePair<string, string>> GetAvailableTimes(ICallMeBackModalWindow datasource);

        bool IsValidFileType(string originalFileName, ICallMeBackModalWindow datasource);

        bool IsValidFileSize(int fileSize, ICallMeBackModalWindow datasource);

        bool Send(CallMeBackModel model, UserCacheDataModel userData, ICallMeBackModalWindow datasource);
    }
}
