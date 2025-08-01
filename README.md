# CodeCart Backend API

A robust and scalable e-commerce backend API built with .NET 8, featuring modern architecture patterns and real-time capabilities.

# Demo
https://youtu.be/Z1qD_A-f-kA?si=9xqBoD5YKjG02Bq1

## 🚀 Features

- **Product Management**: Complete CRUD operations for products with filtering, sorting, and search
- **Shopping Cart**: Redis-powered cart system supporting anonymous and authenticated users
- **Order Processing**: Full order lifecycle with Stripe payment integration
- **Real-time Updates**: SignalR implementation for live payment status updates
- **Authentication & Authorization**: JWT-based auth with role-based access control
- **Security**: Token blacklisting, secure payment processing, and data protection
- **Performance**: Redis caching, SQL Server indexing, and pagination
- **Admin Panel**: Order management and refund capabilities

## 🏗️ Architecture

Built using **Onion Architecture** with the following layers:

- **CodeCart.API**: Web API controllers and configuration
- **CodeCart.Core**: Domain entities, interfaces, and business logic
- **CodeCart.Infrastructure**: Data access, external services, and repositories
- **CodeCart.Service**: Business services and application logic

## 🛠️ Technology Stack

- **.NET 8** - Web API framework
- **Entity Framework Core** - ORM for data access
- **SQL Server** - Primary database
- **Redis** - Caching and cart storage
- **SignalR** - Real-time communication
- **AutoMapper** - Object mapping
- **Stripe** - Payment processing
- **JWT** - Authentication tokens

## 📦 Design Patterns

- **Generic Repository Pattern** - Data access abstraction
- **Unit of Work Pattern** - Transaction management
- **Specification Pattern** - Query composition
- **Dependency Injection** - IoC container usage



### Database
The application uses Entity Framework Code First approach with migrations for database schema management.

### Redis Configuration
Redis is used for:
- Shopping cart storage
- Product caching
- JWT token blacklisting

### Authentication
JWT tokens with configurable expiration and refresh token support. Role-based authorization for admin features.

## 🔐 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Token Blacklisting** - Redis-based token invalidation
- **Role-based Authorization** - Admin and user role management
- **Data Protection** - Sensitive data encryption
- **CORS Configuration** - Cross-origin request handling

## 📈 Performance Optimizations

- **Database Indexing** - Optimized queries with proper indexes
- **Pagination** - Efficient data loading for large datasets
- **Redis Caching** - Product and session caching
- **Async/Await** - Non-blocking operations throughout

## 🔄 Real-time Features

SignalR hubs for:
- Payment status updates
- Order notifications
- Admin dashboard updates
