using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting
{
    public static class Constants
    {
        public static class FileAttributes
        {
            public const string GROUP = "GROUP";
            public const string GROUP_OBLIG = "GROUP_OBLIG";
            public const string OBLIGATORY = "OBLIGATORY";
            public const string PRINTED = "PRINTED";
            public const string SIGN_REQ = "SIGN_REQ";
            public const string TMST_REQ = "TMST_REQ";
            public const string ADDINFO = "ADDINFO";
            public const string MAIN_DOC = "MAIN_DOC";
        }

        public static class SitecorePaths
        {
            public const string LOGIN_TYPES = "/sitecore/content/eContracting2/Settings/LoginTypes";
            public const string PROCESSES = "/sitecore/content/eContracting2/Settings/Processes";
            public const string PROCESS_TYPES = "/sitecore/content/eContracting2/Settings/ProcessTypes";
            public const string DEFINITIONS = "/sitecore/content/eContracting2/Definitions";
        }
    }
}
