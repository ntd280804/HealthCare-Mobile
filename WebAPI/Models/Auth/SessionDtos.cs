namespace WebAPI.Models.Auth
{
    public class SessionInfoDto
    {
        public string Username { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string OracleSid { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class LogoutAllSessionsRequest
    {
        /// <summary>
        /// Nếu false, giữ lại session hiện tại.
        /// </summary>
        public bool IncludeCurrentSession { get; set; } = true;
    }

    public class LogoutPlatformRequest
    {
        public string TargetPlatform { get; set; } = string.Empty;

        /// <summary>
        /// Nếu false, sẽ giữ lại session hiện tại (nếu trùng platform & sessionId).
        /// </summary>
        public bool IncludeCurrentSession { get; set; } = false;
    }
}

