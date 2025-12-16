using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApp.Helpers;
using WebApp.Models;
using WebApp.Models.Auth;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SessionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly OracleClientHelper _oracleClientHelper;

        public SessionController(IHttpClientFactory httpClientFactory, OracleClientHelper oracleClientHelper)
        {
            _httpClient = httpClientFactory.CreateClient("WebApiClient");
            _oracleClientHelper = oracleClientHelper;
        }

        public async Task<IActionResult> Index()
        {
            if (!_oracleClientHelper.TrySetHeaders(_httpClient, out var redirect))
                return redirect;

            var model = new SessionPageViewModel
            {
                CurrentPlatform = HttpContext.Session.GetString("Platform") ?? string.Empty,
                CurrentSessionId = HttpContext.Session.GetString("SessionId") ?? string.Empty
            };

            try
            {
                var resp = await _httpClient.GetAsync("api/session");
                if (resp.IsSuccessStatusCode)
                {
                    var api = await resp.Content.ReadFromJsonAsync<WebApiResponse<List<SessionInfoModel>>>();
                    model.Sessions = api?.Data ?? new List<SessionInfoModel>();
                }
                else
                {
                    TempData["SessionMessage"] = $"Không thể tải danh sách phiên: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["SessionMessage"] = $"Lỗi khi gọi API: {ex.Message}";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutAll(bool includeCurrentSession = true)
        {
            if (!_oracleClientHelper.TrySetHeaders(_httpClient, out var redirect))
                return redirect;

            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/session/logout-all", new
                {
                    includeCurrentSession
                });

                var api = await resp.Content.ReadFromJsonAsync<WebApiResponse<int>>();
                if (resp.IsSuccessStatusCode && api?.Success == true)
                {
                    TempData["SessionMessage"] = $"Đã đăng xuất {api.Data} phiên.";
                }
                else
                {
                    TempData["SessionMessage"] = api?.Error ?? $"Đăng xuất thất bại: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["SessionMessage"] = $"Lỗi khi gọi API: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPlatform(string targetPlatform, bool includeCurrentSession = false)
        {
            if (string.IsNullOrWhiteSpace(targetPlatform))
            {
                TempData["SessionMessage"] = "Vui lòng chọn platform cần đăng xuất.";
                return RedirectToAction(nameof(Index));
            }

            if (!_oracleClientHelper.TrySetHeaders(_httpClient, out var redirect))
                return redirect;

            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/session/logout-platform", new
                {
                    targetPlatform,
                    includeCurrentSession
                });

                var api = await resp.Content.ReadFromJsonAsync<WebApiResponse<int>>();
                if (resp.IsSuccessStatusCode && api?.Success == true)
                {
                    TempData["SessionMessage"] = $"Đã đăng xuất {api.Data} phiên trên {targetPlatform}.";
                }
                else
                {
                    TempData["SessionMessage"] = api?.Error ?? $"Đăng xuất thất bại: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["SessionMessage"] = $"Lỗi khi gọi API: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

