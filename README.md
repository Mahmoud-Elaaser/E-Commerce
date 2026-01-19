# 🛒 E-Commerce Platform

A full-featured e-commerce platform built with **ASP.NET Core 8**, providing a complete shopping experience from product discovery to secure payment processing.

---

## ✨ Features

### 👤 Customer Features
- **Product Discovery**  
  Browse and search through available products
- **Shopping Cart**  
  Add products to cart and manage quantities
- **Order Management**  
  Complete checkout process and track orders
- **Payment Processing**  
  Secure payment integration with Stripe
- **User Authentication**
  - User registration with email confirmation
  - Secure login system
  - Password management (change, forgot, reset) via email

---

### 🛠️ Admin Features
- **Role Management**  
  Assign and remove user roles (admin-only access)

---

## 🧰 Tech Stack

### Core Framework
- **ASP.NET Core 8 Web API** 
- **Entity Framework Core**

---

### Architecture & Patterns
- **Specification Design Pattern** – Flexible querying with reusable specifications
- **Unit of Work Pattern** – Manages database transactions and ensures data consistency
- **Repository Pattern** – Abstracts data access logic

---

### Performance Optimization

- **Redis Caching** - Frequently accessed data is cached to reduce database load
- **Pagination** - Large datasets are paginated to improve response times
- **AutoMapper** - Reduces boilerplate code and improves mapping performance
- **Specification Pattern** - Enables efficient database queries with EF Core

---

### Data & Validation
- **LINQ** – Efficient data querying and manipulation
- **AutoMapper** – Object-to-object mapping for DTOs
- **FluentValidation** – Elegant validation rules for request models

---

### Payment Integration
- **Stripe** – Secure payment processing
- **Stripe Webhooks** – Real-time payment event handling

---

### Additional Features
- **Pagination** – Efficient handling of large datasets
- **Custom Exception Handling** – Centralized error management and user-friendly error responses

---
## 🔮 Future Work

- **Admin Dashboard**
  - View platform statistics and analytics
  - Identify top-selling products and most active sellers
  - Monitor orders, payments, and user activity

- **Stock & Inventory Management**
  - Track product stock levels in real time
  - Automatic stock updates after orders
  - Low-stock notifications for admins
  - Prevent checkout when products are out of stock

- **Advanced Reporting**
  - Sales reports by date, category, or seller
  - Revenue and performance insights

- **Enhanced User Experience**
  - Product recommendations
  - Wishlist functionality
  - Order history analytics
---

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server
- Redis Server
- Stripe Account (for payment processing)

---

## 📦 Installation

### Clone the Repository
```bash
git clone https://github.com/Mahmoud-Elaaser/E-Commerce
cd E-Commerce
```
### Configure Application Settings

Update `appsettings.json` with your configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-database-connection-string",
    "Redis": "your-redis-connection-string"
  },
  "JwtOptions": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "DurationInDays": 1
  },
  "StripeOptions": {
    "PublishableKey": "your-stripe-publishable-key",
    "SecretKey": "your-stripe-secret-key",
    "EndPointSecret": "your-webhook-secret"
  },
   "EmailOptions": {
    "DisplayName": "E-Commerce",
    "Email": "",
    "Host": "smtp.gmail.com",
    "Password": "",
    "Port": "587"
  }
}
```
Apply Database Migrations
```bash
Update-Database
```
Run the Application


