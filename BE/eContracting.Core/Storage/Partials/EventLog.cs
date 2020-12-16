using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;

namespace eContracting.Storage
{
    public partial class EventLog
    {
        public EventLog()
        {
        }

        public EventLog(EventLogModel model)
        {
            this.Event = Enum.GetName(typeof(EVENT_NAMES), model.Event);
            this.Guid = model.Guid;
            this.Message = model.Message;
            this.SessionId = model.SessionId;
            this.Time = model.Time;
            this.Error = model.ErrorMessage;
        }
    }
}
