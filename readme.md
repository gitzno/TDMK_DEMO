# 🚀 Real-time IoT Data & Image Tracking System

Hệ thống thu thập dữ liệu số liệu (Telemetry) và hình ảnh theo thời gian thực, hiển thị biểu đồ Line Chart động sử dụng kiến trúc Event-Driven với .NET 8, SignalR và Nuxt 3.

## 🏗 Kiến trúc Hệ thống (Architecture)

Dự án được xây dựng theo mô hình Microservices/Modular Monolith sẵn sàng cho Docker:

1.  **Ingestion API (.NET 8):** Tiếp nhận dữ liệu số liệu và Upload ảnh.
2.  **SQL Server:** Lưu trữ metadata và lịch sử số liệu.
3.  **SignalR Hub:** Đẩy dữ liệu Real-time xuống Dashboard.
4.  **Nuxt 3 Dashboard:** Hiển thị biểu đồ Line Chart (ECharts/Chart.js) cập nhật trực tiếp.
5.  **Docker Compose:** Đóng gói toàn bộ môi trường (DB, API, Frontend).

---

## 🛠 Công nghệ sử dụng (Tech Stack)

* **Backend:** ASP.NET Core 8.0, Entity Framework Core.
* **Real-time:** Microsoft SignalR.
* **Database:** SQL Server 2022.
* **Frontend:** Nuxt 3, Vue 3, TailwindCSS.
* **DevOps:** Docker, Docker Compose.

---

## 🚀 Hướng dẫn Chạy dự án (Quick Start)

### 1. Yêu cầu hệ thống
* Đã cài đặt **Docker Desktop**.
* Đã cài đặt **Git**.
* Clone code project về máy local.

---

### 2. Các bước triển khai
```bash

# Khởi chạy toàn bộ hệ thống bằng Docker
docker-compose up -d --build