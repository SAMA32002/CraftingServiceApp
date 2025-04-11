# 🛠️ Crafting Services Web Application

A full-featured web application for connecting clients with skilled crafters, built using ASP.NET Core MVC and Web API. It supports user management, service bookings, secure payments via Stripe, and a complete admin panel.

---

## 📌 Features

### 🧑‍🔧 Client & Crafter Portal (ASP.NET Core MVC)
- Register/Login for Clients and Crafters
- Multi-role support with ASP.NET Identity
- Create and manage craft services by category
- Clients can browse, request, and review services
- Escrow-based payment flow using Stripe
- Smart address management (clients can save multiple addresses, filter by nearby crafters)

### 🛠️ Admin Dashboard (AdminLTE + ASP.NET Web API)
- View/Create/Edit/Delete admin users
- Ban/Unban users (clients & crafters)
- Manage services, categories, and posts
- Handle tickets (complaints, suggestions, inquiries — including anonymous)
- Slider management (active images shown on homepage)

---

## 🏗️ Architecture

- **Frontend (Client)**: ASP.NET Core MVC + Bootstrap
- **Admin Panel**: AdminLTE integrated with ASP.NET Web API
- **Backend**:
  - ASP.NET Core Web API
  - Entity Framework Core (Database access)
  - Repository + Unit of Work patterns
- **Authentication**: ASP.NET Identity (Custom Roles: Admin, Client, Crafter)
- **Payments**: Stripe with escrow-like logic
- **Design Patterns**: N-Tier Architecture, Dependency Injection

---

## ⚙️ Technologies Used

| Area        | Stack                                      |
|-------------|--------------------------------------------|
| Language    | C#, Razor                                  |
| Framework   | ASP.NET Core 9.0                           |
| UI          | Bootstrap 5, AdminLTE (v4.0.0-beta3)       |
| DB Access   | Entity Framework Core                      |
| Auth        | ASP.NET Identity + Cookie Authentication   |
| Payments    | Stripe API                                 |
| Dev Tools   | Visual Studio, SQL Server, Git             |

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022+
- SQL Server or LocalDB
- Stripe account (for payment configuration)

### Setup Instructions

1. **Clone the repo**  
   ```bash
   git clone https://github.com/SAMA32002/CraftingServiceApp.git
   cd crafting-services-app
