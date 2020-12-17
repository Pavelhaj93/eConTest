using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Tests
{
    public class MemoryEventLogger : IEventLogger
    {
        public void Add(string sessionId, string guid, EVENT_NAMES eventName)
        {
            
        }

        public void Add(string sessionId, string guid, EVENT_NAMES eventName, string message)
        {
            
        }

        public void Add(EventLogModel eventLog)
        {
            
        }
    }
}
