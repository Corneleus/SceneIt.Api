# SceneIt API

ASP.NET Core backend for SceneIt. The API owns the `MediaItem` library, OMDb proxy integration, import queue state, and import run history.

## Current Functionality

### Media library

- `GET /api/MediaItems` returns only non-deleted media items.
- `GET /api/MediaItems?kind=movie|series|videoGame` filters library reads by normalized media type.
- `GET /api/MediaItems/{id}` returns a single non-deleted media item or `404`.
- `POST /api/MediaItems` creates a media item from a DTO payload.
- Duplicate adds return `409 Conflict`.
- Re-adding a soft-deleted media item restores the existing row instead of creating a second record.
- `PATCH /api/MediaItems/{id}/soft-delete` marks a media item deleted and removes it from normal reads.
- `DELETE /api/MediaItems/{id}` permanently removes the row.

### OMDb integration

- `GET /api/MediaItems/search?query=...` proxies OMDb title search for the frontend.
- `GET /api/MediaItems/search?query=...&kind=...` filters OMDb search results to a section-specific media kind.
- `GET /api/MediaItems/lookup/{imdbId}` proxies OMDb lookup by IMDb ID.
- OMDb responses are normalized before returning or importing:
  - `N/A` values are converted to `null`
  - release dates are parsed into `DateTime?` when possible
- OMDb failures are surfaced as problem responses with an HTTP status code.

### Import queue

- `POST /api/Imports/queue` accepts `{ items: [{ imdbId, title? }] }`.
- Queue submissions trim input values and ignore blank IMDb IDs.
- Duplicate IMDb IDs inside the same request are collapsed before persistence.
- IMDb IDs already present in the import queue are skipped.
- New queue items are enriched from OMDb at queue time when lookup succeeds.
- `GET /api/Imports/queue` returns the queue ordered with `Pending` items first.

### Import runs

- `POST /api/Imports/run` accepts `{ maxCount }`.
- Manual runs only process `Pending` queue items.
- Manual runs are capped at 100 items per batch even if a larger number is requested.
- Each run is recorded before item processing starts.
- Each queue item is processed with per-item error isolation.
- Queue items track attempts, last-attempt time, imported time, and the latest error message.
- `GET /api/Imports/runs` returns run history ordered newest first.

### Import automation

- `ImportAutomationService` and `Imports:Automation` settings exist in the codebase.
- The hosted automation runner is not currently registered in `Program.cs`, so scheduled background imports are disabled at runtime right now.
- The current frontend automation form is therefore session-only UI state and does not persist through this API.

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / LocalDB
- Swagger in development
- xUnit

## Project Structure

- `Program.cs`: DI registration, CORS, Swagger, and app startup
- `Controllers/MediaItemsController.cs`: media-library CRUD plus OMDb search and lookup
- `Controllers/ImportsController.cs`: queue submission, queue reads, manual runs, and run history
- `Services/MediaLibraryService.cs`: media-library rules including duplicate and restore-on-add behavior
- `Services/MediaImportService.cs`: queue persistence, run execution, and queue-item status updates
- `Services/ImportAutomationService.cs`: background automation implementation currently not wired into startup
- `Services/OmdbImportClient.cs`: OMDb HTTP client and response normalization
- `Data/SceneItDbContext.cs`: EF Core model and entity configuration
- `Migrations/`: schema history

## Schema Notes

Current entity sets:

- `MediaItems`
- `Users`
- `UserMediaItems`
- `ImportQueueItems`
- `ImportRuns`

Current migrations in the repo:

- `20260402204927_InitialCreate`
- `20260403034109_AddImportQueue`
- `20260404065811_AddImportQueueMetadata`
- `20260406004225_RenameMovieToMediaItem`

Important model constraints:

- `MediaItems.ImdbId` is unique.
- `ImportQueueItems.ImdbId` is unique.
- `MediaItems.Released` is stored as SQL `date`.
- `ImportQueueItems.Status` is stored as a string conversion of the enum.

Migration note:

- `20260406004225_RenameMovieToMediaItem` is a rename-based migration that preserves existing library data by renaming tables, keys, and foreign-key columns instead of dropping and recreating them.

## Configuration

### Database

The default connection string targets LocalDB:

```json
"DefaultConnection": "Server=(LocalDB)\\MSSQLLocalDB;Database=SceneIt;Trusted_Connection=True;"
```

Apply migrations:

```bash
dotnet ef database update
```

Explicit PowerShell form:

```powershell
dotnet ef database update --project .\SceneIt.Api.csproj --startup-project .\SceneIt.Api.csproj --context SceneItDbContext
```

### OMDb

The API reads OMDb settings from configuration under `Omdb`:

```json
"Omdb": {
  "BaseUrl": "https://www.omdbapi.com/"
}
```

The API key should not be committed to `appsettings.json`.

For local development, `Program.cs` also enables `.NET` user-secrets:

```bash
dotnet user-secrets set "Omdb:ApiKey" "YOUR_OMDB_KEY"
```

For deployed environments, provide the key through secret storage or an environment variable such as `Omdb__ApiKey`.

### CORS

The development CORS policy currently allows:

- `http://localhost:4200`

## Running Locally

Restore, build, and test:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release ..\SceneIt.Api.Tests\SceneIt.Api.Tests.csproj
```

Run the API:

```bash
dotnet run
```

Development launch profiles expose:

- `https://localhost:44383`
- `http://localhost:5078`

Swagger is enabled in Development at `/swagger`.

## API Surface

### MediaItems

- `GET /api/MediaItems`
- `GET /api/MediaItems?kind=movie`
- `GET /api/MediaItems?kind=series`
- `GET /api/MediaItems?kind=videoGame`
- `GET /api/MediaItems/{id}`
- `POST /api/MediaItems`
- `PATCH /api/MediaItems/{id}/soft-delete`
- `DELETE /api/MediaItems/{id}`
- `GET /api/MediaItems/search?query=...`
- `GET /api/MediaItems/search?query=...&kind=series`
- `GET /api/MediaItems/lookup/{imdbId}`

### Imports

- `POST /api/Imports/queue`
- `GET /api/Imports/queue`
- `POST /api/Imports/run`
- `GET /api/Imports/runs`

## Testing

Run tests:

```bash
dotnet test -c Release
```

The committed backend tests currently cover 31 test cases across:

- create, duplicate, restore, soft-delete, and hard-delete media-library service behavior
- queue deduplication and duplicate skipping
- successful import, duplicate import, OMDb null lookup failure, and add failure handling
- run-limit capping at 100 items
- imports controller queue and run responses
- media-items controller conflict, delete, search-failure, and lookup-failure responses
- media-kind matching for legacy and OMDb type values

Current backend test count: 31 passing tests.

## Notes For The Frontend

- The Angular UI expects to call this API at `https://localhost:44383/api` in development.
- Swagger and the API can run at the same time as the Angular dev server.
- The current API does not expose endpoints for saving automation settings; only queueing, running, and reading import state are supported.
- Existing library rows may still contain older type values such as `series` or `game`; `MediaKindMatcher` normalizes these alongside OMDb/IMDb values like `tvSeries` and `videoGame`.
