# E-Hisse Architecture Documentation

## Overview
E-Hisse is restructured following **Clean Architecture** and **Onion Architecture** principles in .NET 10.
The solution guarantees separation of concerns, high maintainability, testability, and strict inward dependency flow.

```
                    ┌─────────────────────────┐
                    │      EHisse.Domain      │
                    │   (Core Entities & Enums)│
                    └────────────▲────────────┘
                                 │
                    ┌────────────┴────────────┐
                    │   EHisse.Application    │
                    │ (Interfaces, DTOs, Logic)│
                    └───────▲───────────▲─────┘
                            │           │
              ┌─────────────┴──┐     ┌──┴────────────────┐
              │EHisse.Persistence│    │EHisse.Infrastructure│
              │(EF Core, Npgsql)│    │(R2, S3, Notifiers)│
              └─────────────▲──┘     └──▲────────────────┘
                            │           │
                    ┌───────┴───────────┴─────┐
                    │      EHisse.WebApi      │
                    │ (Controllers, Swagger)  │
                    └─────────────────────────┘
```

## Solution Projects

### 1. Core Layer
* **`EHisse.Domain`**
  * Target: `net10.0` Class Library
  * Dependencies: **None** (Zero third-party dependencies)
  * Responsibilities: Domain entities (`User`, `SellerProfile`, `Category`, `DeviceBrand`, `DeviceModel`, `SparePart`, `SparePartImage`, `SparePartSaleAudit`, `SparePartReview`, `ActivityLog`), Enums (`UserRole`, `ApprovalStatus`), and domain base abstractions.

* **`EHisse.Application`**
  * Target: `net10.0` Class Library
  * Dependencies: Depends only on `EHisse.Domain`
  * Responsibilities: Application contracts/interfaces (`IStorageService`, `IBlobStorageService`, `INotificationService`, `IWarehouseService`, `IAdminService`, `ICatalogService`), Data Transfer Objects (DTOs), business validation rules (`FluentValidation`), and domain helpers.

### 2. Infrastructure Layer
* **`EHisse.Persistence`**
  * Target: `net10.0` Class Library
  * Dependencies: Depends on `EHisse.Application` and `EHisse.Domain`
  * Responsibilities: Entity Framework Core (`AppDbContext`), PostgreSQL configuration (`Npgsql.EntityFrameworkCore.PostgreSQL`), dynamic query building (`DynamicQueryBuilder`, `SparePartQueryExtensions`), repository/data service implementations, and EF Core migrations.

* **`EHisse.Infrastructure`**
  * Target: `net10.0` Class Library
  * Dependencies: Depends on `EHisse.Application`
  * Responsibilities: External integrations including Cloudflare R2 / AWS S3 (`AmazonS3Client`), Azure Blob Storage (`BlobStorageService`), and notification delivery engines (`LoggingNotificationService`).

### 3. Presentation Layer
* **`EHisse.WebApi`**
  * Target: `net10.0` ASP.NET Core Web API
  * Dependencies: Depends on `EHisse.Application`, `EHisse.Persistence`, and `EHisse.Infrastructure`
  * Responsibilities: API controllers (`AuthController`, `AdminController`, `WarehouseController`, `HomeController`, `SparePartsController`, `FileUploadController`, `AjaxController`, `ImageController`, `PublicApiController`), Global Exception Handling Middleware, Swagger/OpenAPI documentation, CORS, and clean Dependency Injection bootstrapping.

## Dependency Injection Registration
Services are registered cleanly via extension methods in `Program.cs`:
```csharp
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
```

## Building the Solution
```bash
dotnet build EHisse.sln
```
