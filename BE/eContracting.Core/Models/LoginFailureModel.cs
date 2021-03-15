using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eContracting.Models
{
    public class LoginFailureModel
    {
        public string Guid { get; set; }

        public string SessionId { get; set; }

        public string BrowserAgent { get; set; }

        public DateTime Timestamp { get; set; }

        public LOGIN_STATES LoginState { get; set; }

        public LoginTypeModel LoginType { get; set; }

        public bool IsBirthdateValid { get; set; }

        public bool IsValueValid { get; set; }

        public string CampaignCode { get; set; }

        public LoginFailureModel(string guid, string sessionId)
        {
            this.Guid = guid;
            this.SessionId = sessionId;
            this.Timestamp = DateTime.UtcNow;
        }
    }
}
