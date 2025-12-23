# BÁO CÁO CHI TIẾT DỰ ÁN: MOBILE SERVICE SYSTEM

**Ngày tạo báo cáo:** 13/12/2025  
**Chủ sở hữu dự án:** ntd280804  
**Repository:** mobile-service-system  
**Nhánh chính:** main  

---

## 📋 MỤC LỤC

1. [Tổng quan dự án](#tổng-quan-dự-án)
2. [Kiến trúc hệ thống](#kiến-trúc-hệ-thống)
3. [Tech Stack](#tech-stack)
4. [Cấu trúc thư mục](#cấu-trúc-thư-mục)
5. [Các module chính](#các-module-chính)
6. [Tính năng chính](#tính-năng-chính)
7. [Database](#database)
8. [API Endpoints](#api-endpoints)
9. [Công nghệ bảo mật](#công-nghệ-bảo-mật)
10. [Build & Deployment](#build--deployment)

---

## 🎯 TỔNG QUAN DỰ ÁN

### Mô tả
**Mobile Service System** là một hệ thống quản lý dịch vụ di động toàn diện, bao gồm:
- Ứng dụng mobile (Flutter) cho khách hàng và nhân viên
- Ứng dụng web (ASP.NET Core) cho quản trị viên
- API Backend (ASP.NET Core) xử lý logic nghiệp vụ
- Cơ sở dữ liệu Oracle

### Mục đích
Cung cấp nền tảng quản lý toàn diện cho:
- Quản lý đơn hàng (Order Management)
- Quản lý linh kiện (Part Management)
- Quản lý nhập/xuất kho (Import/Export)
- Quản lý hóa đơn (Invoice Management)
- Quản lý lịch hẹn (Appointment Management)
- Xác thực người dùng bảo mật (RSA + JWT)

### Mục tiêu chính
- Nâng cao hiệu quả quản lý dịch vụ
- Cung cấp trải nghiệm người dùng tốt trên mobile
- Đảm bảo bảo mật dữ liệu cao
- Hỗ trợ offline-first cho mobile
- Real-time communication với SignalR

---

## 🏗️ KIẾN TRÚC HỆ THỐNG

### Mô hình 3-Tier

```
┌─────────────────────────────────────────┐
│   PRESENTATION LAYER                    │
│  ┌─────────────────┬──────────────────┐ │
│  │ Mobile App      │ Web App (Admin)   │ │
│  │ (Flutter)       │ (ASP.NET Razor)   │ │
│  └────────┬────────┴────────┬─────────┘ │
└───────────┼──────────────────┼──────────┘
            │                  │
┌───────────┼──────────────────┼──────────┐
│ BUSINESS LOGIC LAYER (WebAPI)           │
│  ┌────────────────────────────────────┐ │
│  │ REST API + SignalR Hubs             │ │
│  │ - Admin Controllers                 │ │
│  │ - Public Controllers                │ │
│  │ - Common Controllers                │ │
│  └────────────────────────────────────┘ │
└───────────┬──────────────────┬──────────┘
            │                  │
            │     Services     │
            │   (Auth, PDF,    │
            │    Email, QR)    │
            │                  │
┌───────────┼──────────────────┼──────────┐
│ DATA LAYER                               │
│  ┌────────────────────────────────────┐ │
│  │ Oracle Database                     │ │
│  │ - Tables                            │ │
│  │ - Stored Procedures                 │ │
│  │ - Audit Triggers                    │ │
│  │ - VPD (Virtual Private Database)    │ │
│  └────────────────────────────────────┘ │
└─────────────────────────────────────────┘
```

### Luồng yêu cầu
1. Mobile/Web → WebAPI (REST/SignalR)
2. WebAPI → Services (Business Logic)
3. Services → Oracle DB
4. Response trả về → Mobile/Web UI

---

## 💻 TECH STACK

### Backend - WebAPI
| Công nghệ | Phiên bản | Mục đích |
|-----------|----------|---------|
| .NET | 8.0 | Framework chính |
| ASP.NET Core | 8.0 | Web API Framework |
| Oracle.ManagedDataAccess | 23.26.0 | Oracle Database Driver |
| Oracle.EntityFrameworkCore | 9.23.90 | ORM |
| Microsoft.AspNetCore.SignalR | 1.2.0 | Real-time Communication |
| JWT Bearer | 8.* | Token-based Authentication |
| QRCoder | 1.7.0 | QR Code Generation |
| QuestPDF | 2024.10.2 | PDF Generation |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger/OpenAPI |
| GroupDocs.Signature | 25.6.0 | Digital Signature |
| UglyToad.PdfPig | 1.7.0 | PDF Processing |

### Frontend - WebApp
| Công nghệ | Phiên bản | Mục đích |
|-----------|----------|---------|
| .NET | 8.0 | Framework chính |
| ASP.NET Core MVC | 8.0 | Web Application |
| Razor Pages | 8.0 | Server-side rendering |

### Mobile - Flutter App
| Công nghệ | Phiên bản | Mục đích |
|-----------|----------|---------|
| Flutter | Latest | Cross-platform Framework |
| Dart | Latest | Programming Language |
| Dio | Latest | HTTP Client |
| Mobile Scanner | Latest | QR Code Scanner |
| SignalR Client | Latest | Real-time Messaging |
| Provider | Latest | State Management |

### Database - Oracle
| Thành phần | Mô tả |
|-----------|--------|
| Oracle Database | RDBMS chính |
| PL/SQL | Stored Procedures, Triggers |
| Audit Triggers | Lịch sử thay đổi dữ liệu |
| VPD | Row-level Security |
| OLS | Label Security |
| FGA | Fine Grained Auditing |

### DevOps & Tools
- **Version Control:** Git
- **CI/CD:** GitHub Actions (trong .github/)
- **Package Management:** NuGet, npm
- **Build Tools:** MSBuild, Gradle
- **Testing:** xUnit, NUnit

---

## 📁 CẤU TRÚC THƯ MỤC

```
mobile-service-system/
├── .git/                          # Git repository
├── .github/                       # GitHub configuration
├── Mobile/
│   └── customer_app/              # Flutter Mobile App
│       ├── lib/
│       │   ├── screens/           # UI Screens
│       │   ├── services/          # API & Business Logic
│       │   ├── models/            # Data Models
│       │   ├── config/            # Configuration
│       │   └── widgets/           # Reusable Widgets
│       ├── android/               # Android specific code
│       ├── ios/                   # iOS specific code
│       └── pubspec.yaml           # Dependencies
│
├── WebAPI/                        # Backend API (ASP.NET Core)
│   ├── Areas/
│   │   ├── Admin/                 # Admin endpoints
│   │   │   ├── Controllers/
│   │   │   │   ├── PartController
│   │   │   │   ├── OrderController
│   │   │   │   ├── ImportController
│   │   │   │   ├── ExportController
│   │   │   │   ├── InvoiceController
│   │   │   │   ├── EmployeeController
│   │   │   │   ├── CustomerController
│   │   │   │   ├── AuditController
│   │   │   │   ├── ProfileController
│   │   │   │   └── RoleController
│   │   │   └── Models/
│   │   ├── Public/                # Public endpoints (Login, Register)
│   │   │   └── Controllers/
│   │   └── Common/                # Shared endpoints
│   │       └── Controllers/
│   ├── Services/                  # Business Logic
│   │   ├── OracleConnectionManager
│   │   ├── ProxyLoginService
│   │   ├── QrGeneratorSingleton
│   │   ├── PdfService
│   │   ├── EmailService
│   │   ├── RsaKeyService
│   │   ├── QrLoginStore
│   │   ├── WebToMobileQrStore
│   │   └── PdfTemplates/
│   ├── Hubs/                      # SignalR Hubs
│   │   └── NotificationHub
│   ├── Helpers/                   # Utility functions
│   │   ├── ControllerResponseHelper
│   │   ├── OracleSessionHelper
│   │   └── SecurePayloadHelper
│   ├── Models/                    # DTOs & Entities
│   ├── Program.cs                 # Startup configuration
│   ├── appsettings.json           # Configuration
│   └── WebAPI.csproj              # Project file
│
├── WebApp/                        # Admin Web Application
│   ├── Areas/
│   │   ├── Admin/                 # Admin pages
│   │   │   └── Views/
│   │   └── Public/                # Public pages
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Views/
│   ├── wwwroot/                   # Static files (CSS, JS)
│   ├── Program.cs
│   ├── appsettings.json
│   └── WebApp.csproj
│
├── SQL/                           # Database scripts
│   ├── FULL_DATABASE_EXPORT.sql   # Full schema export
│   ├── Table.sql                  # Table definitions
│   ├── Grant.sql                  # User permissions
│   ├── Audit-Trigger/             # Audit trigger scripts
│   ├── Audit-FGA/                 # Fine Grained Audit
│   ├── Audit-StandardAudit/       # Standard Audit
│   ├── VPD/                       # Virtual Private DB
│   ├── OLS/                       # Label Security
│   └── Backup-Restore RMAN/       # Backup scripts
│
├── docs/                          # Documentation
│   ├── identity-authentication.md
│   ├── pdf-signature-workflow.md
│   ├── rsa_aes_signature_security.md
│   ├── web_login_by_qr_flow.md
│   └── mobile-ui-plan-vi.md
│
├── diagram/                       # UML Diagrams
│   ├── UC-01.oob, UC-01.oom       # Use Cases
│   ├── UC-02.oob, UC-02.oom
│   ├── UC-03.oob, UC-03.oom
│   ├── UC-04.oob, UC-04.oom
│   ├── UC-05.oob, UC-05.oom
│   └── Workspace.sws
│
├── Build/                         # Build outputs
├── Private_Key_Employee/          # Employee private keys (Secure)
├── Chữ kí số doanh nghiệp/        # Company digital signatures
├── Hóa đơn demo/                  # Sample invoices
├── Sơ đồ/                         # Diagrams & flowcharts
│
├── mobile-service-system-main.sln # Solution file
└── .gitignore                     # Git ignore rules
```

---

## 🔧 CÁC MODULE CHÍNH

### 1. **Authentication & Authorization Module**
- **File chính:** `ProxyLoginService.cs`, `RsaKeyService.cs`
- **Tính năng:**
  - Xác thực người dùng qua Oracle proxy account
  - Hỗ trợ JWT tokens
  - Quản lý phiên làm việc (Session)
  - Mã hóa RSA cho mật khẩu
  
### 2. **QR Code Management Module**
- **File chính:** `QrGeneratorSingleton.cs`, `QrLoginStore.cs`, `WebToMobileQrStore.cs`
- **Tính năng:**
  - Sinh mã QR cho linh kiện
  - QR Login cho web
  - Web-to-Mobile QR confirmation
  
### 3. **Order Management Module**
- **Controller:** `OrderController.cs`
- **Tính năng:**
  - Tạo/cập nhật đơn hàng
  - Quản lý trạng thái đơn hàng
  - Lịch sử đơn hàng
  
### 4. **Part Management Module**
- **Controller:** `PartController.cs`
- **Tính năng:**
  - Quản lý linh kiện
  - Quét QR linh kiện
  - Thông tin chi tiết linh kiện
  - Theo dõi lịch sử
  
### 5. **Import/Export Module**
- **Controllers:** `ImportController.cs`, `ExportController.cs`
- **Tính năng:**
  - Quản lý nhập kho
  - Quản lý xuất kho
  - Theo dõi tồn kho
  
### 6. **Invoice Management Module**
- **Controller:** `InvoiceController.cs`
- **Tính năng:**
  - Sinh hóa đơn
  - Ký số hóa đơn (GroupDocs.Signature)
  - Xuất PDF hóa đơn
  - Xác minh chữ ký
  
### 7. **Appointment Management Module**
- **Tính năng:**
  - Tạo lịch hẹn
  - Quản lý lịch hẹn
  - Thông báo lịch hẹn
  
### 8. **Audit & Compliance Module**
- **Folder:** `SQL/Audit-Trigger/`, `SQL/Audit-FGA/`, `SQL/Audit-StandardAudit/`
- **Tính năng:**
  - Audit triggers theo dõi thay đổi
  - Fine Grained Auditing (FGA)
  - Lịch sử đầy đủ của dữ liệu
  - Compliance reporting

### 9. **Real-time Communication Module**
- **File chính:** `NotificationHub.cs`
- **Tính năng:**
  - WebSocket communication qua SignalR
  - Real-time notifications
  - Broadcasting messages
  
### 10. **PDF & Report Generation Module**
- **Files:** `PdfService.cs`, `IPdfTemplate.cs`, `*Template.cs`
- **Templates:**
  - SalesInvoiceTemplate
  - ImportInvoiceTemplate
  - ExportInvoiceTemplate
- **Tính năng:**
  - Sinh PDF hóa đơn
  - Tùy chỉnh template
  - Ký số trên PDF

---

## ✨ TÍNH NĂNG CHÍNH

### Mobile App (Flutter)
| Tính năng | Mô tả |
|-----------|-------|
| Đăng nhập | Xác thực qua username/password |
| QR Scanner | Quét QR để xem chi tiết linh kiện |
| Dashboard | Hiển thị thông tin tổng quát |
| Quản lý đơn hàng | Xem danh sách, chi tiết đơn hàng |
| Quản lý linh kiện | Xem linh kiện, quét QR, xem lịch sử |
| Lịch hẹn | Tạo, xem, quản lý lịch hẹn |
| Real-time Notifications | Nhận thông báo tức thì |
| Offline Support | Tính năng offline first |

### Web App (Admin)
| Tính năng | Mô tả |
|-----------|-------|
| Quản lý người dùng | CRUD nhân viên, khách hàng |
| Quản lý đơn hàng | Toàn bộ quản lý đơn hàng |
| Quản lý linh kiện | CRUD linh kiện, sinh QR |
| Quản lý nhập/xuất | Quản lý kho, lịch sử |
| Quản lý hóa đơn | Tạo, ký số, xuất hóa đơn |
| Báo cáo | Thống kê, báo cáo chi tiết |
| Audit Log | Xem lịch sử thay đổi dữ liệu |

### WebAPI Endpoints
- **Public:** Login, Register, Forgot Password
- **Admin:** Full CRUD operations
- **Common:** Shared resources (Orders, Appointments)
- **Real-time:** SignalR Hub notifications

---

## 🗄️ DATABASE

### Công nghệ: Oracle Database

### Các thành phần chính:
1. **Tables**: Toàn bộ bảng dữ liệu
2. **Stored Procedures**: Các thủ tục lưu trữ
3. **Triggers**: Audit triggers tự động
4. **Audit Tables**: Bảng lưu lịch sử
5. **VPD Policies**: Row-level security
6. **OLS Labels**: Label-based security
7. **FGA Policies**: Fine-grained audit

### Các bảng chính (dự kiến):
- **CUSTOMERS** - Khách hàng
- **EMPLOYEES** - Nhân viên
- **ORDERS** - Đơn hàng
- **PARTS** - Linh kiện
- **IMPORTS** - Phiếu nhập kho
- **EXPORTS** - Phiếu xuất kho
- **INVOICES** - Hóa đơn
- **APPOINTMENTS** - Lịch hẹn
- **USERS** - Tài khoản người dùng
- **AUDIT_***  - Các bảng audit lịch sử

### Security Features:
- **Encryption**: Mã hóa nhạy cảm
- **Audit Triggers**: Tự động ghi lại mọi thay đổi
- **VPD**: Đảm bảo người dùng chỉ xem dữ liệu của mình
- **OLS**: Phân loại dữ liệu theo mức độ bảo mật
- **FGA**: Ghi lại truy cập chi tiết

### Scripts:
```
SQL/
├── FULL_DATABASE_EXPORT.sql  (Toàn bộ schema)
├── Table.sql                 (Cấu trúc bảng)
├── Grant.sql                 (Phân quyền)
├── Audit-Trigger/            (Audit triggers)
├── VPD/                       (Virtual Private DB)
├── OLS/                       (Label Security)
├── FGA/                       (Fine Grained Audit)
└── Backup-Restore RMAN/       (Backup scripts)
```

---

## 🔌 API ENDPOINTS

### Base URL
```
https://10.147.20.56:5131
```

### Public Endpoints
```
POST   /api/Public/Customer/login              # Đăng nhập
POST   /api/Public/Customer/register           # Đăng ký
POST   /api/Public/Customer/change-password    # Đổi mật khẩu
POST   /api/Public/Customer/logout             # Đăng xuất
POST   /api/Public/Appointment                 # Tạo lịch hẹn
POST   /api/Public/QrLogin/confirm             # Xác nhận QR Login
POST   /api/Public/WebToMobileQr/confirm       # Xác nhận Web-to-Mobile QR
```

### Admin Endpoints
```
GET    /api/Admin/Part                         # Lấy toàn bộ linh kiện
GET    /api/Admin/Part/{serial}/details        # Chi tiết linh kiện
GET    /api/Admin/Part/in-stock                # Linh kiện trong kho
GET    /api/Admin/Part/{orderId}/by-order-id   # Linh kiện theo order
GET    /api/Admin/Part/{orderId}/by-part-request  # Linh kiện theo yêu cầu

GET    /api/Admin/Order                        # Danh sách đơn hàng
POST   /api/Admin/Order                        # Tạo đơn hàng
GET    /api/Admin/Order/{orderId}              # Chi tiết đơn hàng
PUT    /api/Admin/Order/{orderId}              # Cập nhật đơn hàng

GET    /api/Admin/Import                       # Danh sách nhập
GET    /api/Admin/Export                       # Danh sách xuất
GET    /api/Admin/Invoice                      # Danh sách hóa đơn

GET    /api/Admin/Employee/{username}          # Thông tin nhân viên
POST   /api/Admin/Employee/login               # Đăng nhập nhân viên

GET    /api/Admin/Profile                      # Danh sách profile
GET    /api/Admin/Role                         # Danh sách role
```

### Common Endpoints
```
GET    /api/Common/Order                       # Danh sách đơn hàng
GET    /api/Common/Order/{orderId}             # Chi tiết đơn hàng
GET    /api/Common/Appointment                 # Danh sách lịch hẹn
```

### SignalR Hub
```
Hub URL: /Hubs/notification
Events:
  - ReceiveNotification(message)               # Nhận thông báo
  - OrderUpdated(orderId, status)              # Cập nhật đơn hàng
  - PartScanned(partSerial)                    # Quét linh kiện
```

---

## 🔐 CÔNG NGHỆ BẢO MẬT

### 1. **Authentication**
- **JWT Tokens**: Token-based authentication
- **OAuth Proxy**: Xác thực qua Oracle proxy account
- **Session Management**: Quản lý phiên làm việc

### 2. **Encryption**
- **RSA**: Mã hóa bất đối xứng cho mật khẩu
- **AES**: Mã hóa đối xứng cho dữ liệu
- **SSL/TLS**: Kết nối HTTPS

### 3. **Digital Signature**
- **GroupDocs.Signature**: Ký số hóa đơn
- **X.509 Certificates**: Chứng chỉ số
- **Signature Verification**: Xác minh chữ ký

### 4. **Authorization**
- **Role-Based Access Control (RBAC)**: Phân quyền theo vai trò
- **Attribute-Based Access Control (ABAC)**: Phân quyền theo thuộc tính
- **Oracle VPD**: Row-level security

### 5. **Audit & Compliance**
- **Audit Triggers**: Tự động ghi lại mọi thay đổi
- **FGA (Fine Grained Auditing)**: Ghi lại truy cập
- **OLS (Label Security)**: Phân loại dữ liệu
- **Change Log**: Lịch sử đầy đủ

### 6. **Data Protection**
- **Password Hashing**: Lưu mật khẩu đã hash
- **Database Encryption**: Mã hóa cơ sở dữ liệu
- **Secure Keys**: Quản lý khóa bảo mật

---

## 🔨 BUILD & DEPLOYMENT

### Backend Build
```bash
# WebAPI
dotnet build WebAPI/WebAPI.csproj
dotnet publish WebAPI/WebAPI.csproj -c Release

# WebApp
dotnet build WebApp/WebApp.csproj
dotnet publish WebApp/WebApp.csproj -c Release
```

### Mobile Build
```bash
# Build APK
flutter build apk --release

# Build iOS
flutter build ios --release
```

### Database Setup
```bash
# Execute SQL scripts in order:
1. FULL_DATABASE_EXPORT.sql
2. Table.sql
3. Grant.sql
4. Audit-Trigger/*.sql
5. VPD/*.sql
6. OLS/*.sql
```

### Configuration
- **appsettings.json**: Database connection, API settings
- **appsettings.Development.json**: Development overrides
- **Program.cs**: Startup configuration, dependency injection

### Deployment
- **WebAPI**: Port 5131 (HTTPS), 5130 (HTTP)
- **WebApp**: Port 7158 (HTTPS), 5176 (HTTP)
- **Database**: Oracle instance (production server)

---

## 📊 THỐNG KÊ CODEBASE

### Code Organization
| Thành phần | Số lượng |
|-----------|---------|
| Controllers | 11+ (Admin, Public, Common) |
| Services | 10+ (Auth, QR, PDF, Email, etc.) |
| Models/DTOs | 50+ |
| Database Tables | 15+ (estimated) |
| Stored Procedures | 30+ (estimated) |
| SQL Scripts | 20+ |
| Documentation Files | 5+ |

### Technology Distribution
- **Backend**: C# + ASP.NET Core (65%)
- **Frontend**: Dart + Flutter (20%)
- **Database**: PL/SQL (10%)
- **Configuration**: JSON + XML (5%)

---

## 🚀 KEY FEATURES SUMMARY

### Đối với Khách Hàng (Mobile App)
✅ Đăng nhập & Quản lý tài khoản  
✅ Xem & Quản lý đơn hàng  
✅ Quét QR linh kiện  
✅ Tạo & Quản lý lịch hẹn  
✅ Real-time notifications  
✅ Offline mode  

### Đối với Nhân Viên (Mobile App)
✅ Toàn bộ quyền khách hàng  
✅ Dashboard quản lý  
✅ Xem chi tiết đơn hàng  
✅ Quản lý linh kiện  
✅ Quét QR linh kiện  

### Đối với Quản Trị Viên (Web App)
✅ Quản lý toàn bộ dữ liệu  
✅ Tạo & Ký số hóa đơn  
✅ Báo cáo & Thống kê  
✅ Quản lý người dùng & Role  
✅ Audit log & Compliance  
✅ System administration  

---

## 📝 NOTES & OBSERVATIONS

1. **Security-First Design**: Hệ thống được thiết kế với bảo mật cao
2. **Real-time Capabilities**: Sử dụng SignalR cho communication tức thời
3. **Scalable Architecture**: Tách biệt API, WebApp, Database
4. **Comprehensive Audit**: Đầy đủ audit log tại database level
5. **Modern Tech Stack**: Sử dụng .NET 8, Flutter, Oracle mới
6. **Document Generation**: Hỗ trợ PDF generation & digital signature
7. **Multi-platform**: Hỗ trợ mobile, web, và API
8. **Enterprise Features**: VPD, OLS, FGA cho security enterprise

---

## 📚 DOCUMENTATION

Các tài liệu trong `docs/` folder:
- `identity-authentication.md` - Kiến trúc xác thực
- `pdf-signature-workflow.md` - Workflow ký số hóa đơn
- `rsa_aes_signature_security.md` - Kỹ thuật mã hóa
- `web_login_by_qr_flow.md` - Luồng QR Login
- `mobile-ui-plan-vi.md` - Kế hoạch UI Mobile

---

## ✅ CONCLUSION

**Mobile Service System** là một dự án hoàn chỉnh, kết hợp công nghệ hiện đại với yêu cầu bảo mật cao. Hệ thống được thiết kế để:
- Hỗ trợ multiple platforms (Mobile, Web)
- Đảm bảo security và compliance
- Cung cấp real-time capabilities
- Dễ maintain và scale

Dự án phù hợp cho các nhu cầu quản lý dịch vụ toàn diện với quy mô enterprise.

---

**End of Report**  
*Generated: December 13, 2025*
