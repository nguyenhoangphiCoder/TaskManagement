# Hệ Thống Quản Lý Công Việc (Task Management System)

Một ứng dụng Quản lý công việc và Bảng Kanban mạnh mẽ, hoạt động theo thời gian thực (real-time) được xây dựng dựa trên Clean Architecture, CQRS, .NET 9 và Next.js.

## Tính Năng Chính

- **Không gian làm việc & Dự án**: Tổ chức công việc bằng cách tạo nhiều không gian làm việc (Workspaces) và dự án (Projects).
- **Bảng Kanban**: Kéo thả công việc giữa các trạng thái khác nhau một cách dễ dàng.
- **Quản Lý Công Việc**: Chi tiết công việc bao gồm Tiêu đề, Mô tả, Mức độ ưu tiên, Ngày đến hạn và Trạng thái.
- **Cộng Tác**: Giao việc cho các thành viên trong nhóm, thêm bình luận, và theo dõi tiến độ bằng danh sách công việc phụ (Checklists).
- **Cập Nhật Theo Thời Gian Thực**: Thấy ngay các thay đổi do thành viên khác thực hiện mà không cần tải lại trang (Được hỗ trợ bởi SignalR).
- **Giao Diện Tương Thích**: Giao diện người dùng hiện đại, lấy cảm hứng từ phong cách glassmorphism được thiết kế bằng Tailwind CSS.

## Công Nghệ Sử Dụng

### Backend
- **Framework**: .NET 9 Web API
- **Kiến Trúc**: Clean Architecture & CQRS Pattern (MediatR)
- **Cơ Sở Dữ Liệu**: Entity Framework Core 9 (EF Core)
- **Thời Gian Thực**: ASP.NET Core SignalR
- **Xác Thực Dữ Liệu**: FluentValidation
- **Bảo Mật**: JWT Bearer Tokens

### Frontend
- **Framework**: Next.js (React)
- **Giao Diện**: Tailwind CSS
- **HTTP Client**: Axios
- **Thời Gian Thực**: @microsoft/signalr
- **Kéo Thả**: dnd-kit

## Cấu Trúc Dự Án

```
.
├── src/
│   ├── TaskManagement.API/           # Web API Entry Point & SignalR Hubs
│   ├── TaskManagement.Application/   # CQRS Handlers, DTOs và Interfaces
│   ├── TaskManagement.Domain/        # Entities, Enums và Logic cốt lõi
│   ├── TaskManagement.Infrastructure/# EF Core DB Context và Repositories
│   └── task-management-web/          # Ứng dụng Next.js Frontend
└── README.md
```

## Hướng Dẫn Cài Đặt

### Yêu Cầu Hệ Thống

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (phiên bản v18 hoặc mới hơn)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (hoặc LocalDB)

### 1. Khởi Chạy Backend

Di chuyển vào thư mục API và khởi động server:

```bash
cd src/TaskManagement.API
dotnet restore
dotnet build
dotnet run
```
*Backend API mặc định sẽ chạy tại `http://localhost:5041`. Bạn có thể truy cập Swagger UI tại `http://localhost:5041/swagger`.*

### 2. Khởi Chạy Frontend

Mở một terminal mới, di chuyển vào thư mục frontend, cài đặt thư viện và khởi động development server:

```bash
cd src/task-management-web
npm install
npm run dev
```
*Ứng dụng Next.js sẽ hoạt động tại `http://localhost:3000`.*

## Biến Môi Trường

Đảm bảo rằng bạn đã cấu hình các biến môi trường và chuỗi kết nối cần thiết trước khi chạy ứng dụng.

**Backend (`appsettings.json`):**
- Chuỗi kết nối cơ sở dữ liệu (Database Connection String)
- Cấu hình JWT (Secret, Issuer, Audience)

**Frontend (`.env.local`):**
- API Base URL: `NEXT_PUBLIC_API_URL=http://localhost:5041/api/v1`
- SignalR Base URL: `NEXT_PUBLIC_SIGNALR_URL=http://localhost:5041/hubs`

## Giấy Phép

Dự án này được phân phối dưới giấy phép MIT.
