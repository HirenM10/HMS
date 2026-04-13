# Clinical Hospital Management Backend

Modular monolith backend for a Clinical module built with Clean Architecture, CQRS, MediatR, EF Core, SQL Server, JWT authentication, Serilog, API versioning, ProblemDetails, and automated tests.

## Solution layout

```text
src/
  Clinical.Domain
  Clinical.Application
  Clinical.Infrastructure
  Clinical.API
tests/
  Clinical.UnitTests
  Clinical.IntegrationTests
```

## Prerequisites

- .NET SDK 10.0.103 or later
- SQL Server instance for local/dev runtime

## Setup

1. Restore packages:

```bash
dotnet restore --configfile NuGet.Config
```

2. Run the API:

```bash
dotnet run --project src/Clinical.API
```

3. Open Swagger:

- `https://localhost:7043/swagger`
- `http://localhost:5207/swagger`

## Authentication

Use the seeded users from `appsettings.json` with `POST /api/v1/auth/login`.

- `admin@clinical.local` / `Admin123!`
- `doctor@clinical.local` / `Doctor123!`
- `reception@clinical.local` / `Reception123!`

## Test commands

```bash
dotnet test --configfile NuGet.Config
```

