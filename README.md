# localCRM

A production-grade, multi-user Customer Relationship Management (CRM) system built with .NET 8, SQLite, and Blazor WebAssembly.

## 🚀 Overview

localCRM is designed as a modular, secure, and performant application following Clean Architecture principles. It provides a hybrid API approach with both REST and GraphQL endpoints, ensuring flexibility for various client needs.

## 🛠️ Technology Stack

- **Backend:** .NET 8.0, ASP.NET Core Web API
- **Persistence:** SQLite via Entity Framework Core 8.0
- **GraphQL:** HotChocolate v14
- **Messaging:** MediatR for decoupling application logic
- **Mapping:** AutoMapper
- **Validation:** FluentValidation
- **Security:** ASP.NET Core Identity with custom JWT & Refresh Token rotation
- **Frontend:** Blazor WebAssembly (WASM) with MudBlazor v7 UI components

## 🏗️ Architecture

The project follows the **Clean Architecture** pattern:

- `src/backend/LocalCRM.Domain`: Core entities, value objects, and domain logic.
- `src/backend/LocalCRM.Application`: DTOs, MediatR handlers, and application services.
- `src/backend/LocalCRM.Infrastructure`: SQLite persistence, Identity services, and JWT implementation.
- `src/backend/LocalCRM.API`: REST Controllers and HotChocolate GraphQL resolvers.
- `src/frontend/LocalCRM.Blazor`: Blazor WASM client application.

## 🛡️ Key Features

- **Global Security:** All endpoints are secured by default. Administrative features are restricted to the `Administrator` role.
- **Refresh Token Lifecycle:** Robust session management with token rotation, reuse detection, and session-wide revocation.
- **Audit Logging:** Detailed tracking of data modifications and security events.
- **Soft Delete:** Business entities support soft-delete and restore operations.
- **Optimistic Concurrency:** Data integrity enforced via `updated_at` timestamps.
- **Priority Search:** Global search across all CRM entities with engagement prioritization.
- **Dynamic Dashboard:** Real-time metrics for both business operations and system health.

## 🏁 Getting Started

### Prerequisites

- .NET 8.0 SDK

### Initialization

Initialize the SQLite database and seed the default administrator account:

```bash
cd src/backend/LocalCRM.API
dotnet run -- --init --password=YourSecurePassword123!
```

### Running the Application

1. **Start the API:**
   ```bash
   cd src/backend/LocalCRM.API
   dotnet run
   ```
2. **Start the Blazor Frontend:**
   ```bash
   cd src/frontend/LocalCRM.Blazor
   dotnet run
   ```

## 📖 Documentation

Detailed specifications and architectural guides can be found in the `.docs` directory.
