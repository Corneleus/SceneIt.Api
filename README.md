# SceneIt API

ASP.NET Core backend for SceneIt. The API owns the movie library, EF Core data model, import queue, import runs, and OMDb-backed import/search integration.

## Current Functionality

- Movie library CRUD surface for the frontend.
- DTO-based movie API responses and requests.
- Duplicate movie adds return `409 Conflict`.
- Soft delete and hard delete endpoints.
- Re-adding a soft-deleted movie restores the existing row instead of failing on the unique IMDb constraint.
- Manual import queue endpoints for queue inspection and batch execution.
- Hosted background scheduler for automated import runs while the API is running.
- OMDb-backed search and lookup endpoints used by the frontend add-movie flow.
- EF Core migrations as the source of truth for schema.

## Project State Checkpoint

- Backend repo: `/mnt/c/Users/Corne/source/repos/scene-it-backend/SceneIt.Api`
- EF Core migrations are the schema source of truth.
- Current migrations in the repo:
  - `20260402204927_InitialCreate`
  - `20260403034109_AddImportQueue`
- Current schema represented by those migrations includes:
  - `Movies`
  - `Users`
  - `UserMovies`
  - `ImportQueueItems`
  - `ImportRuns`
  - `__EFMigrationsHistory`
- Backend features currently in place:
  - movie DTO-based API
  - duplicate add returns `409 Conflict`
  - soft delete and hard delete endpoints
  - manual import endpoints:
    - `POST /api/imports/queue`
    - `GET /api/imports/queue`
    - `POST /api/imports/run`
    - `GET /api/imports/runs`
  - hosted scheduler for automated imports
  - frontend-safe OMDb proxy endpoints:
    - `GET /api/Movies/search?query=...`
    - `GET /api/Movies/lookup/{imdbId}`
- Scheduler defaults in `appsettings.json`:
  - `Enabled = true`
  - `RunOnStartup = true`
  - `IntervalMinutes = 1440`
  - `MaxImportsPerDay = 100`
  - `MaxCountPerRun = 100`
- Important note:
  - automatic imports only happen while the backend is running
  - if imports must continue while the backend is offline, switch to external scheduling later
- Current verification status:
  - `dotnet build -c Release` passes
  - `dotnet test -c Release` passes with 16 tests
- Files of interest:
  - `Controllers/ImportsController.cs`
  - `Services/MovieImportService.cs`
  - `Services/ImportAutomationService.cs`
- Likely next discussions:
  - keep hosted scheduler vs move to external scheduling
  - admin UI for import queue and import runs
  - stronger import failure reporting and retry strategy

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core with SQL Server / LocalDB
- Swagger in development
- xUnit tests

## Project Structure

- `Controllers/MoviesController.cs`: movie CRUD plus OMDb proxy endpoints
- `Controllers/ImportsController.cs`: import queue and run endpoints
- `Services/MovieService.cs`: movie library rules, duplicate handling, soft delete restore behavior
- `Services/MovieImportService.cs`: queue processing and import run persistence
- `Services/ImportAutomationService.cs`: hosted scheduled import runner
- `Data/SceneItDbContext.cs`: EF Core model
- `Migrations/`: schema history

## Local Setup

### 1. Configure the database

The default connection string targets LocalDB:

```json
"DefaultConnection": "Server=(LocalDB)\\MSSQLLocalDB;Database=SceneIt;Trusted_Connection=True;"
```

Apply migrations:

```bash
dotnet ef database update
```

### 2. Configure OMDb with user-secrets

The OMDb API key is no longer stored in source control. For local development, use .NET user-secrets.

From this project directory:

```bash
dotnet user-secrets set "Omdb:ApiKey" "YOUR_OMDB_KEY"
```

Optional verification:

```bash
dotnet user-secrets list
```

### 3. Run the API

```bash
dotnet run
```

Development launch URLs:

- `https://localhost:44383`
- `http://localhost:5078`

Swagger is enabled in development at `/swagger`.

## API Surface

### Movies

- `GET /api/Movies`
- `GET /api/Movies/{id}`
- `POST /api/Movies/add`
- `PATCH /api/Movies/{id}/soft-delete`
- `DELETE /api/Movies/{id}`
- `GET /api/Movies/search?query=...`
- `GET /api/Movies/lookup/{imdbId}`

### Imports

- `POST /api/imports/queue`
- `GET /api/imports/queue`
- `POST /api/imports/run`
- `GET /api/imports/runs`

## Import Automation

Import automation is configured in `appsettings.json` under `Imports:Automation`.

Current defaults:

- `Enabled = true`
- `RunOnStartup = true`
- `IntervalMinutes = 1440`
- `MaxImportsPerDay = 100`
- `MaxCountPerRun = 100`

Important behavior:

- Automated imports only run while the backend process is up.
- The hosted service is appropriate for local/dev or simple always-on hosting.
- If imports must continue while the API is offline, use an external scheduler later.

## Reliability Notes

- Import runs are created and persisted before batch work begins.
- Each queue item is handled with per-item error isolation so one failed import does not discard the entire run.
- Import failures are recorded on the queue item with run counts updated as processing continues.

## Build and Test

Build:

```bash
dotnet build -c Release
```

Run tests:

```bash
dotnet test -c Release
```

Current backend test coverage includes:

- duplicate add handling
- restore-on-add for soft-deleted movies
- soft delete and hard delete behavior
- import queue duplicate skipping
- import run success, duplicate, and failure states
- import run persistence when add/import work throws

## Notes for the Frontend

The Angular app at `http://localhost:4200` is allowed by CORS in development and is expected to call this API at `https://localhost:44383/api`.
