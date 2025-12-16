using System.Collections.Generic;

namespace WebApp.Models.Auth
{
    public class SessionInfoModel
    {
        public string Username { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string OracleSid { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class SessionPageViewModel
    {
        public List<SessionInfoModel> Sessions { get; set; } = new();
        public string CurrentPlatform { get; set; } = string.Empty;
        public string CurrentSessionId { get; set; } = string.Empty;
    }
}

