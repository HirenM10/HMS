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

Before logging in with seeded users, set these environment variables in your shell:

```powershell
$env:CLINICAL_SEED_ADMIN_SECRET="<choose-a-local-password>"
$env:CLINICAL_SEED_DOCTOR_SECRET="<choose-a-local-password>"
$env:CLINICAL_SEED_RECEPTION_SECRET="<choose-a-local-password>"
```

3. Open Swagger:

- `https://localhost:7043/swagger`
- `http://localhost:5207/swagger`

## Authentication

Use the seeded users from `appsettings.json` with `POST /api/v1/auth/login`. Their sign-in secrets are loaded from environment variables so credentials are not stored in source control.

- `admin@clinical.local` / `%CLINICAL_SEED_ADMIN_SECRET%`
- `doctor@clinical.local` / `%CLINICAL_SEED_DOCTOR_SECRET%`
- `reception@clinical.local` / `%CLINICAL_SEED_RECEPTION_SECRET%`

## Test commands

```bash
dotnet restore --configfile NuGet.Config
dotnet test --no-restore
```
