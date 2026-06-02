# SmartExpenseTracker API

> Serverless expense tracking API built with .NET 9, Azure Functions, and CI/CD automation via GitHub Actions.

[![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=flat-square&logo=githubactions&logoColor=white)](https://github.com)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Azure Functions](https://img.shields.io/badge/Azure%20Functions-V4-0062AD?style=flat-square&logo=azurefunctions&logoColor=white)](https://azure.microsoft.com/en-us/products/functions)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Azure%20SQL-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)](https://azure.microsoft.com/en-us/products/azure-sql)

---

## Overview

SmartExpenseTracker is a production-grade serverless API designed as a portfolio project to demonstrate real-world CI/CD practices, Clean Architecture, and cloud deployment on Azure.

The system allows registering and querying expenses through a RESTful API running on Azure Functions, with full automated pipelines for DEV and PROD environments.

---

## Architecture

```
┌─────────────────────────────────────────────────┐
│                  GitHub Actions                 │
│                                                 │
│  develop → DEV Pipeline  → Azure Functions DEV  │
│  main    → PROD Pipeline → Azure Functions PROD │
└─────────────────────────────────────────────────┘
                      │
                      ▼
        ┌─────────────────────────┐
        │   Azure Functions API   │
        │   (.NET 9 Isolated)     │
        │                        │
        │  POST /api/expenses     │
        │  GET  /api/expenses     │
        │  GET  /api/expenses/{id}│
        │  GET  /api/healthcheck  │
        └───────────┬─────────────┘
                    │
                    ▼
        ┌─────────────────────────┐
        │    Azure SQL Server     │
        │     SmartExpenseDb      │
        └─────────────────────────┘
```

### Project Structure

```
SmartExpenseTracker.sln
├── SmartExpenseTracker.Domain/          # Entities, interfaces
│   ├── Entities/Expense.cs
│   └── Interfaces/IExpenseRepository.cs
├── SmartExpenseTracker.Application/     # Business logic
│   ├── Interfaces/IExpenseService.cs
│   └── Services/ExpenseService.cs
├── SmartExpenseTracker.Infrastructure/  # Data access (Dapper)
│   └── Repositories/ExpenseRepository.cs
├── SmartExpenseTracker.Api/             # Azure Functions
│   ├── Functions/ExpensesFunction.cs
│   ├── HealthCheck.cs
│   └── Program.cs
└── SmartExpenseTracker.Tests/           # Unit tests (xUnit + Moq)
```

---

## Tech Stack

| Layer       | Technology                                 |
| ----------- | ------------------------------------------ |
| Runtime     | .NET 9                                     |
| Compute     | Azure Functions V4 — Isolated Worker Model |
| Data Access | Dapper                                     |
| Database    | Azure SQL Server                           |
| Logging     | Application Insights (structured logging)  |
| CI/CD       | GitHub Actions                             |
| Cloud       | Microsoft Azure                            |
| Testing     | xUnit + Moq                                |

### Architectural Patterns

- **Clean Architecture** — Domain / Application / Infrastructure / Api separation
- **Repository Pattern** — abstracted data access behind interfaces
- **Dependency Injection** — all services registered in Program.cs
- **Environment-based configuration** — DEV and PROD isolated by Azure App Settings

---

## CI/CD Pipelines

Two independent pipelines control deployment to separate environments:

| Pipeline | Trigger           | Target                         |
| -------- | ----------------- | ------------------------------ |
| DEV      | Push to `develop` | `func-smartexpensetracker-dev` |
| PROD     | Push to `main`    | `func-smartexpensetracker`     |

Each pipeline runs: **build → unit tests → publish → deploy**.

Deployment uses Azure Service Principals with least-privilege access per environment.

---

## Endpoints

| Method | Route                | Description            |
| ------ | -------------------- | ---------------------- |
| GET    | `/api/healthcheck`   | Health status          |
| POST   | `/api/expenses`      | Register a new expense |
| GET    | `/api/expenses`      | List all expenses      |
| GET    | `/api/expenses/{id}` | Get expense by ID      |

### Sample Request

```http
POST /api/expenses
Content-Type: application/json

{
  "description": "Team lunch",
  "amount": 450.00,
  "category": "Food"
}
```

### Sample Response

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Team lunch",
  "amount": 450.0,
  "category": "Food",
  "createdAt": "2026-06-01T18:00:00Z"
}
```

---

## Running Locally

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- SQL Server (local or Docker)

### Setup

1. Clone the repository:

```bash
git clone https://github.com/<your-username>/SmartExpenseTracker.git
cd SmartExpenseTracker
```

2. Configure `local.settings.json` in `SmartExpenseTracker.Api/`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "SqlConnectionString": "Server=localhost,1433;Database=SmartExpenseDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

3. Run the API:

```bash
cd SmartExpenseTracker.Api
func start
```

4. Run the tests:

```bash
dotnet test
```

---

## Azure Infrastructure

| Resource             | Name                           |
| -------------------- | ------------------------------ |
| Resource Group       | `rg-smartexpensetracker`       |
| Function App (PROD)  | `func-smartexpensetracker`     |
| Function App (DEV)   | `func-smartexpensetracker-dev` |
| SQL Server           | `sql-smartexpense`             |
| Database             | `SmartExpenseDb`               |
| Application Insights | `appi-smartexpensetracker`     |

---

## What I Learned

This project was built as a structured learning path toward .NET Solution Architect. Key takeaways:

- **CI/CD is a practice, not a file** — understanding the why behind pipelines changes how you build them
- **Isolated Worker model** gives full .NET control over DI, middleware, and configuration
- **Dapper over ORM** for Azure Functions Consumption Plan — no persistent connections, no overhead
- **Environment separation** from day one prevents painful refactors later
- **Structured logging** with Application Insights makes production debugging actually useful

---

## Author

**Irving Saul Quiroz**  
.NET Developer — 13 years of experience  
Transitioning to .NET Solution Architect

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-0A66C2?style=flat-square&logo=linkedin)](https://linkedin.com/in/irving-saul-quiroz-a73776a2)
[![GitHub](https://img.shields.io/badge/GitHub-Profile-181717?style=flat-square&logo=github)](https://github.com/Iquiroz10)
