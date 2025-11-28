# E-Commerce Microservices – Small project

This solution includes **two .NET 8 microservices** using **Clean Architecture**, **EF Core**, **SQL Server**, and **Redis Streams** for event-driven communication.

## Services Overview

| Service              | Responsibility                                                            | Database      | Port |
| -------------------- | ------------------------------------------------------------------------- | ------------- | ---- |
| **ProductService**   | CRUD for Products + publishes `ProductCreatedEvent`                       | `ProductDb`   | 7xxx |
| **InventoryService** | CRUD for Inventory + auto-creates initial stock (100) on product creation | `InventoryDb` | 5xxx |

### Event Flow

`ProductService` publishes `ProductCreatedEvent` → consumed by `InventoryService` via Redis Streams with consumer groups, pending message handling, and dead-letter support.

## Architecture

```
Src
├── ProductService
│   ├── API
│   ├── Application
│   ├── Domain
│   └── Infrastructure
├── InventoryService
│   ├── API
│   ├── Application
│   ├── Domain
│   └── Infrastructure
└── SharedContracts
    ├── Dtos
    └── Events
```

## Tech Stack

* .NET 8
* ASP.NET Core Minimal APIs + Controllers
* EF Core + SQL Server
* Redis Streams (StackExchange.Redis)
* Swagger / OpenAPI
* Environment-based configuration
* BackgroundService for robust Redis consumption

## Running Locally

1. Clone the repo:

```bash
git clone git@github.com:abderafi3/Ecommerce-Microservices.git
```

2. Open in Visual Studio 2022+
3. Set **Multiple Startup Projects** → Start `ProductService` and `InventoryService`
4. Press **F5**

Swagger URLs:

* ProductService: `https://localhost:7xxx/swagger`
* InventoryService: `https://localhost:5xxx/swagger`

### Test Flow

1. **POST** `/api/products` → creates product and publishes `ProductCreatedEvent`
2. `InventoryService` auto-creates inventory with `Quantity = 100`

## Highlights

* Clean Architecture
* Idempotent event handling
* Scoped DbContext with async/await
* Redis Streams with retries, pending message recovery, dead-letter queue
* Environment-based config
* Ready for production extensions: CQRS, MediatR, Docker/K8s, Polly, OpenTelemetry

---

