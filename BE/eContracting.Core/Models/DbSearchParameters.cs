using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class DbSearchParameters
    {
        public string Key { get; set; }

        public string Guid { get; set; }

        public string SessionId { get; set; }

        public DbSearchParameters(string key, string guid, string sessionId)
        {
            this.Key = key;
            this.Guid = guid;
            this.SessionId = sessionId;
        }
    }
}
