using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebAPI.Helpers;
using WebAPI.Models;
using WebAPI.Models.Auth;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/session")]
    public class SessionController : ControllerBase
    {
        private readonly OracleConnectionManager _connectionManager;
        private readonly OracleSessionHelper _sessionHelper;
        private readonly ILogger<SessionController> _logger;

        public SessionController(
            OracleConnectionManager connectionManager,
            OracleSessionHelper sessionHelper,
            ILogger<SessionController> logger)
        {
            _connectionManager = connectionManager;
            _sessionHelper = sessionHelper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<ApiResponse<List<SessionInfoDto>>> GetMySessions()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized(ApiResponse<List<SessionInfoDto>>.Fail("Missing username in token"));

            var sessions = _connectionManager.GetConnectionsByUsername(username)
                .Select(s => new SessionInfoDto
                {
                    Username = s.Username,
                    Platform = s.Platform,
                    SessionId = s.SessionId,
                    OracleSid = s.OracleSid,
                    State = s.State
                })
                .ToList();

            return Ok(ApiResponse<List<SessionInfoDto>>.Ok(sessions));
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> LogoutAll([FromBody] LogoutAllSessionsRequest? request)
        {
            if (!_sessionHelper.TryGetSession(HttpContext, out var username, out var platform, out var sessionId))
                return Unauthorized(ApiResponse<int>.Fail("Missing Oracle session headers"));

            bool includeCurrent = request?.IncludeCurrentSession ?? true;
            var removed = await _connectionManager.RemoveAllConnectionsForUser(username, includeCurrent ? null : sessionId);

            _logger.LogInformation("User {Username} logged out all sessions (includeCurrent: {IncludeCurrent}) removed {Count}",
                username, includeCurrent, removed);

            return Ok(ApiResponse<int>.Ok(removed));
        }

        [HttpPost("logout-platform")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> LogoutByPlatform([FromBody] LogoutPlatformRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.TargetPlatform))
                return BadRequest(ApiResponse<int>.Fail("targetPlatform is required"));

            if (!_sessionHelper.TryGetSession(HttpContext, out var username, out var currentPlatform, out var sessionId))
                return Unauthorized(ApiResponse<int>.Fail("Missing Oracle session headers"));

            var removed = await _connectionManager.RemoveConnectionsByPlatform(
                username,
                request.TargetPlatform,
                request.IncludeCurrentSession ? null : sessionId);

            _logger.LogInformation(
                "User {Username} logged out platform {TargetPlatform}, includeCurrent: {IncludeCurrent}, removed {Count}, currentPlatform: {CurrentPlatform}, currentSession: {SessionId}",
                username,
                request.TargetPlatform,
                request.IncludeCurrentSession,
                removed,
                currentPlatform,
                sessionId);

            return Ok(ApiResponse<int>.Ok(removed));
        }
    }
}

