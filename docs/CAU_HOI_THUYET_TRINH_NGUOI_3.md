# THUYẾT TRÌNH - NGƯỜI 3
## Chủ đề: DB Security / Audit / Backup

---

# PHẦN 1: KẾ HOẠCH THUYẾT TRÌNH

## 📌 TỔNG QUAN (Thời lượng đề xuất: ~25 phút)

| Nội dung | Thư mục nguồn | Thời lượng |
|----------|---------------|------------|
| VPD/OLS | `SQL/VPD/*` | 10-12 phút |
| FGA + Trigger Before/After | `SQL/Audit-Trigger/`, `SQL/Audit-FGA/` | 8-10 phút |
| Sao lưu/Phục hồi RMAN | `SQL/Backup-Restore RMAN/` | 5-7 phút |

---

## 📋 PHẦN 1.1: VPD (Virtual Private Database)

### 1.1.1 Giới thiệu VPD (2 phút)
- **VPD là gì**: Row-Level Security tự động thêm predicate vào WHERE
- **File context**: `SQL/VPD/init/01_app_context.sql`
- **Package APP_CTX_PKG**: `set_role()`, `set_emp()`, `set_customer()`, `set_username()`

### 1.1.2 Bảng tổng hợp phân quyền (5 phút)

| Bảng | ADMIN | TIEPTAN | THUKHO | KITHUATVIEN | KHACHHANG |
|------|-------|---------|--------|-------------|-----------|
| CUSTOMER | ✅ Full | ✅ Full | ❌ | ❌ | 🔒 Cá nhân |
| CUSTOMER_APPOINTMENT | ✅ Full | ✅ Full | ❌ | ❌ | 🔒 Cá nhân |
| EMPLOYEE | ✅ Full | ✅ Full | 🔒 Cá nhân | 🔒 Cá nhân | ❌ |
| ORDERS | ✅ Full | ✅ Full | ❌ | 🔒 Handler | 🔒 Cá nhân |
| INVOICE | ✅ Full | ✅ Full | ❌ | ❌ | 🔒 Cá nhân |
| PART | ✅ Full | ❌ | ✅ Full | ✅ Full | ❌ |
| PART_REQUEST | ✅ Full | ❌ | ✅ Full | 🔒 Cá nhân | ❌ |
| STOCK_* | ✅ Full | ❌ | ✅ Full | ❌ | ❌ |

**Ghi chú:**
- ✅ Full: Xem tất cả dữ liệu (`1=1`)
- 🔒 Cá nhân: Chỉ xem dữ liệu của mình
- 🔒 Handler: Xem đơn hàng mình xử lý
- ❌: Không có quyền (`1=0`)

### 1.1.3 Demo VPD (3 phút)
- Giới thiệu các file: `01_*_vpd_function.sql`, `02_*_vpd_add_policy.sql`
- Demo set context và query:
```sql
EXEC APP_CTX_PKG.set_role('ROLE_KITHUATVIEN');
EXEC APP_CTX_PKG.set_emp(5);
SELECT * FROM ORDERS; -- Chỉ thấy đơn hàng mình xử lý
```

---

## 📋 PHẦN 1.2: KIỂM TOÁN (AUDIT)

### 1.2.1 Ba loại Audit (2 phút)

| Loại | File | Mục đích |
|------|------|----------|
| Standard Audit | `SQL/Audit-StandardAudit/init/init.sql` | Audit cơ bản theo statement |
| FGA | `SQL/Audit-FGA/init/init.sql` | Audit chi tiết 17 bảng |
| Trigger Before/After | `SQL/Audit-Trigger/init/*` | Capture old/new values |

### 1.2.2 Trigger Before/After (5 phút)
- **Bảng log**: `audit_alert_log` (file `02_audit_alert_pkg.sql`)
- **Package**: `audit_dml_pkg` (file `04_audit_with_before_after.sql`)
- **Triggers**: file `05_create_all_audit_triggers.sql`
- **Capture**: `dml_type`, `old_values`, `new_values`, `changed_columns`

### 1.2.3 So sánh FGA vs Trigger (2 phút)

| Tính năng | FGA | Trigger |
|-----------|-----|---------|
| Before/After values | ❌ | ✅ |
| Performance | Tốt | Chậm hơn |
| Cấu hình | Dễ | Phức tạp |

**Kết luận**: Dùng cả hai - FGA detect sự kiện, Trigger capture chi tiết

---

## 📋 PHẦN 1.3: RMAN BACKUP/RESTORE

### 1.3.1 Các file script (3 phút)
| File | Chức năng |
|------|-----------|
| `run_rman_backup.sh` | Backup PDB + archivelog |
| `run_rman_restore.sh` | Restore với PITR support |
| `crontab.sh` | Tự động backup lúc 2:30 sáng |

### 1.3.2 Demo commands (3 phút)
```bash
# Backup
BACKUP DATABASE PLUS ARCHIVELOG;

# Restore PITR
SET UNTIL TIME '2025-12-15 14:30:00';
RESTORE PLUGGABLE DATABASE ORCLPDB1;
RECOVER PLUGGABLE DATABASE ORCLPDB1;
```

---
---

# PHẦN 2: CÂU HỎI VÀ TRẢ LỜI

## 📗 10 CÂU HỎI DỄ

### VPD (1-4)

**1. VPD là gì và mục đích sử dụng trong dự án?**
> VPD (Virtual Private Database) là cơ chế Row-Level Security của Oracle, tự động thêm điều kiện WHERE vào câu query để kiểm soát quyền truy cập dữ liệu theo từng dòng.

**2. Application Context `APP_CTX` lưu trữ những thông tin gì?**
> Lưu 4 thông tin: `ROLE_NAME` (vai trò), `EMP_ID` (mã nhân viên), `CUSTOMER_PHONE` (SĐT khách hàng), `USERNAME` (tên đăng nhập).

**3. Có bao nhiêu role trong hệ thống? Kể tên.**
> 5 roles: ROLE_ADMIN, ROLE_TIEPTAN, ROLE_THUKHO, ROLE_KITHUATVIEN, ROLE_KHACHHANG.

**4. Role nào có quyền xem tất cả dữ liệu bảng CUSTOMER?**
> ROLE_ADMIN và ROLE_TIEPTAN (return `1=1`).

### Audit (5-7)

**5. Hệ thống sử dụng những loại audit nào?**
> 3 loại: Standard Audit, Fine-Grained Audit (FGA), và Trigger Before/After.

**6. Bảng `audit_alert_log` dùng để làm gì?**
> Lưu trữ log audit với thông tin: user, thời gian, bảng bị thay đổi, loại thao tác (INSERT/UPDATE/DELETE), giá trị cũ/mới.

**7. FGA audit những loại thao tác nào trên các bảng?**
> INSERT, UPDATE, DELETE với `audit_trail = DB + EXTENDED`.

### RMAN (8-10)

**8. RMAN là gì?**
> Recovery Manager - công cụ backup/restore của Oracle Database.

**9. Script backup RMAN chạy tự động lúc mấy giờ?**
> 2:30 sáng hàng ngày (cấu hình trong crontab: `30 2 * * *`).

**10. Lệnh RMAN nào dùng để backup database kèm archivelog?**
> `BACKUP DATABASE PLUS ARCHIVELOG;`

---

## 📕 10 CÂU HỎI KHÓ

### VPD (1-4)

**1. Giải thích cách VPD function `ORDERS_VPD_PREDICATE` phân quyền cho ROLE_KITHUATVIEN?**
> Trả về `HANDLER_EMP = <v_emp>` - kỹ thuật viên chỉ xem được đơn hàng mà họ là người xử lý (HANDLER_EMP = EMP_ID của họ trong context).

**2. Tại sao trong VPD function cần kiểm tra `v_role IS NULL` và return `1=0`?**
> Để đảm bảo security - nếu chưa set context (session mới hoặc bypass), mặc định chặn tất cả truy cập. Tránh trường hợp user truy cập khi chưa được xác thực.

**3. Trong `CUSTOMER_VPD_PREDICATE`, tại sao cần dùng `REPLACE(v_cus,'''','''''')` khi tạo predicate?**
> Để escape ký tự quote (`'`) trong giá trị phone, tránh SQL Injection. Ví dụ: `0911'--` sẽ thành `0911''--`.

**4. So sánh VPD với GRANT quyền truyền thống. Khi nào nên dùng VPD?**
> GRANT chỉ control object-level (SELECT/INSERT trên bảng). VPD control row-level (chỉ xem dữ liệu của mình). Dùng VPD khi cần multi-tenant, phân quyền theo dữ liệu (khách hàng chỉ xem đơn của mình).

### Audit (5-7)

**5. So sánh FGA và Trigger Audit. Tại sao dự án dùng cả hai?**
> - FGA: Nhẹ, dễ cấu hình, nhưng không capture được giá trị before/after
> - Trigger: Capture được old/new values dạng JSON, nhưng chậm hơn
> - Dùng cả hai: FGA detect sự kiện nhanh, Trigger lưu chi tiết cho forensic/compliance

**6. Package `audit_dml_pkg` capture những thông tin gì từ Application Context?**
> Capture `APP_CTX.ROLE_NAME`, `APP_CTX.EMP_ID`, `APP_CTX.CUSTOMER_PHONE` để biết ai (role nào, employee nào) thực hiện thao tác - liên kết với VPD context.

**7. Giải thích cách trigger audit capture `changed_columns` khi UPDATE?**
> So sánh từng cột `:OLD.column != :NEW.column`, nếu khác thì thêm tên cột vào danh sách. Ví dụ: `IF (:OLD.FULL_NAME != :NEW.FULL_NAME) THEN v_changed_cols := v_changed_cols || 'FULL_NAME,'; END IF;`

### RMAN (8-10)

**8. Giải thích sự khác biệt giữa restore thông thường và Point-in-Time Recovery (PITR)?**
> - Restore thông thường: Khôi phục về backup gần nhất, mở PDB bình thường
> - PITR: Khôi phục về thời điểm cụ thể bằng `SET UNTIL TIME`, phải mở PDB với `RESETLOGS` vì timeline thay đổi

**9. Tại sao script restore cần `ALTER PLUGGABLE DATABASE CLOSE IMMEDIATE` trước khi restore?**
> PDB phải ở trạng thái MOUNT (không OPEN) để RMAN có thể restore/recover datafiles. Nếu PDB đang OPEN, các file đang được sử dụng và không thể ghi đè.

**10. Trong môi trường Container Database (CDB), giải thích cách backup script kết nối đến PDB cụ thể thay vì CDB?**
> Sử dụng EZCONNECT với PDB service name: `rman target user/pwd@//host:port/ORCLPDB1`. Điều này cho phép backup riêng PDB mà không ảnh hưởng CDB hay các PDB khác.

---

## 📚 TÀI LIỆU THAM KHẢO

| Nội dung | File |
|----------|------|
| VPD Context | `SQL/VPD/init/01_app_context.sql` |
| VPD Functions | `SQL/VPD/*/01_*_vpd_function.sql` |
| VPD Policies | `SQL/VPD/*/02_*_vpd_add_policy.sql` |
| FGA Init | `SQL/Audit-FGA/init/init.sql` |
| Audit Table & Package | `SQL/Audit-Trigger/init/02_audit_alert_pkg.sql` |
| Audit Triggers | `SQL/Audit-Trigger/init/05_create_all_audit_triggers.sql` |
| RMAN Backup | `SQL/Backup-Restore RMAN/run_rman_backup.sh` |
| RMAN Restore | `SQL/Backup-Restore RMAN/run_rman_restore.sh` |
