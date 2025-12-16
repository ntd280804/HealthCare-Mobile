## Hệ thống bảo hành điện thoại

Ứng dụng đa nền tảng (WebAPI .NET + WebApp MVC + Flutter mobile) hỗ trợ đăng nhập an toàn, quản lý kho/đơn hàng/hóa đơn, ký số và kiểm toán trên Oracle Database.

### Cấu trúc nhanh
- `WebAPI/`: API .NET 8, JWT, SignalR, ký số.
- `WebApp/`: MVC web/desktop client.
- `Mobile/HealthCare_MobileAPP/`: Flutter client.
- `Mã nguồn SQL/`: script DB (VPD/OLS, Audit, Backup/Restore).
- `Sơ đồ/`: tài liệu phân tích, sơ đồ xác thực/QR/VPD.
- `Đóng gói/`: bản dựng thử (apk, WebAPI/WebAPP publish).

### Đối chiếu tiêu chí & flow (kèm dẫn chứng)
- **Đăng nhập (mobile + web/desktop)**  
  Flow: client gửi username/password → API hash & tạo Oracle session → set role/context → trả JWT + sessionId.  
  Mã: `WebAPI/Areas/Public/Controllers/CustomerController.cs` (khách) và `WebAPI/Areas/Admin/Controllers/EmployeeController.cs` (nhân viên).  
  QR / Web-to-Mobile: quét mã → `QrLoginController.cs` hoặc `WebToMobileQrController.cs` lấy trạng thái/confirm → sinh token qua `ProxyLoginService.cs` với store `QrLoginStore.cs` / `WebToMobileQrStore.cs`.


- **Giới hạn phiên / single session / timeout**  
  Flow: mỗi lần login gọi `OracleConnectionManager.CreateConnection` sẽ `RemoveAllConnections` cùng (username, platform) để tránh đa phiên; keep-alive qua `TryRefreshSession`; nếu session chết thì `OracleSessionHelper.HandleSessionKilled` + SignalR `NotificationHub` push `ForceLogout`.  
  Mã: `WebAPI/Services/OracleConnectionManager.cs`, `WebAPI/Helpers/OracleSessionHelper.cs`, `WebAPI/Hubs/NotificationHub.cs`.


- **Chính sách mật khẩu / profile (idle_time, connect_time, failed attempts, lock, inactive)**  
  Flow: admin tạo/đọc/sửa profile Oracle và gán user → khi user login, Oracle áp policy và trả lỗi 28000/28001 nếu khóa/hết hạn → API trả thông báo tương ứng.  
  Mã: `WebAPI/Areas/Admin/Controllers/ProfileController.cs` (create/update/assign/read). Handling lỗi tại `CustomerController.Login` và `EmployeeController.Login`.


- **Áp dụng >=3 thuộc tính profile**  
  Thuộc tính: `IDLE_TIME`, `CONNECT_TIME`, `FAILED_LOGIN_ATTEMPTS`, `PASSWORD_LOCK_TIME`, `INACTIVE_ACCOUNT_TIME` được đọc/gán qua `ProfileController` (các thủ tục `APP.GET_ALL_PROFILES_WITH_LIMITS`, `APP.CREATE_PROFILE`, `APP.UPDATE_PROFILE`, `APP.ASSIGN_PROFILE_TO_USER`).


- **Đăng xuất (mobile + web/desktop)**  
  Flow: client gọi `/logout` → `OracleSessionHelper.HandleSessionKilled` xóa session, dọn HttpSession, remove Oracle conn, phát `ForceLogout`.  
  Mã: `CustomerController.Logout`, `EmployeeController.Logout`, `OracleSessionHelper.cs`.


- **Đăng ký / đổi mật khẩu**  
  Flow: khách đăng ký `CustomerController.Register`; đổi mật khẩu `ChangePassword` yêu cầu header Oracle session hợp lệ. Nhân viên login/đổi mật khẩu dùng payload mã hóa RSA/AES hybrid qua `SecurePayloadHelper.cs` + `RsaKeyService.cs`.  
  Mã: `CustomerController.cs`, `EmployeeController.cs`, `Helpers/SecurePayloadHelper.cs`.


- **Quản lý kho (xuất/nhập, kiểm kê, tìm kiếm QR)**  
  Flow: WebApp call API quản lý nhập (`ImportController`), xuất (`ExportController`), linh kiện (`PartController`, `PartrequestController`), đơn hàng (`OrderController`), hóa đơn (`InvoiceController`). QR tạo/đọc qua `QrGeneratorSingleton.cs`.  
  Mã: trong `WebAPI/Areas/Admin/Controllers/*`.


- **Hóa đơn & ký số**  
  Flow: sinh PDF template (`Services/PdfTemplates/*`), ký số bằng chứng thư số PFX qua `Services/SignatureService.cs`, lưu lại bằng stored procedure (BLOB).  
  Mã: `WebAPI/Areas/Admin/Controllers/InvoiceController.cs`, `WebAPI/Services/PdfService.cs`, `WebAPI/Services/SignatureService.cs`.


- **Phân quyền + VPD/OLS**  
  Flow: ứng dụng set `APP_CTX` (role, emp, customer phone) rồi Oracle VPD predicate lọc dữ liệu theo role/bản ghi.  
  Mã/Script: `Mã nguồn SQL/VPD/` (ví dụ `Customer/CUSTOMER_VPD_POLICY.md`, `Stock/STOCK_VPD_POLICY.md`, `Order/ORDERS_VPD_POLICY.md`); OLS mẫu ở `Mã nguồn SQL/OLS/`.


- **Kiểm toán (FGA, trigger)**  
  Flow: bật FGA + trigger before/after để log INSERT/UPDATE/DELETE vào `audit_alert_log`, có capture before/after.  
  Script: `Mã nguồn SQL/Audit-Trigger/README.md`, `README_BEFORE_AFTER.md`, các script `03_*_audit_trigger.sql`; FGA init ở `Mã nguồn SQL/Audit-FGA/init/init.sql`.


- **Sao lưu / phục hồi**  
  Flow: dùng RMAN script cron `run_rman_backup.sh` và restore `run_rman_restore.sh`.  
  Vị trí: `Mã nguồn SQL/Backup-Restore RMAN/`.
  
- **Tài liệu tham khảo**  
  Phân tích yêu cầu/chức năng chi tiết: `A16_NguyenPhuongHac.pdf`; sơ đồ quy trình: thư mục `Sơ đồ/` (xác thực, QR, VPD, mô hình lớp).

### Chạy nhanh (dev)
1) **WebAPI**: cập nhật chuỗi kết nối Oracle trong `WebAPI/appsettings.Development.json`, chạy `dotnet run` (Kestrel https 5131, CORS mở rộng).  
2) **WebApp**: cấu hình API base URL trong `WebApp/appsettings.Development.json`, `dotnet run`.  
3) **Mobile**: Flutter, cập nhật endpoint trong `Mobile/HealthCare_MobileAPP/lib` (các service), `flutter run`.  
4) **DB**: chạy các script trong `Mã nguồn SQL/` theo nhu cầu (init app context, VPD, audit, backup).

### Ghi chú bảo mật
- JWT cấu hình tại `WebAPI/Program.cs` (issuer/audience/key).  
- Payload nhạy cảm (login nhân viên) được mã hóa RSA/AES hybrid qua `SecurePayloadHelper.cs` + `RsaKeyService.cs`.  
- Oracle session headers (`X-Oracle-*`) được kiểm tra trong `OracleSessionHelper.cs` trước khi thao tác dữ liệu.

