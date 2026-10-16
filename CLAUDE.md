# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

DominoTracker — an ASP.NET Core Web API (.NET 10) for tracking domino matches, teams, wars (best-of-N series between two teams), players, and seasons. SQL Server via EF Core. No test project exists yet. No frontend in this repo (CORS is configured for a separate frontend at `http://localhost:3000`).

## Commands

Build / run (from repo root, `DominoTracker.slnx`):
```
dotnet build
dotnet run --project DominoTracker.WebAPI
```

EF Core migrations (run from repo root; the Infrastructure project holds `Migrations/`, but startup project is WebAPI):
```
dotnet ef migrations add <Name> --project DominoTracker.Infrastructure --startup-project DominoTracker.WebAPI
dotnet ef database update --project DominoTracker.Infrastructure --startup-project DominoTracker.WebAPI
```

Swagger UI is available at the app root when running in development (`app.UseSwaggerUI()` is unconditional in `Program.cs`).

There is no test project in the solution currently — don't assume `dotnet test` works until one is added.

## Architecture

Standard layered solution, one project per layer, referenced top-down:

- **DominoTracker.Domain** — plain entity classes only (`Player`, `Team`, `Match`, `War`, `WarMatch`, `Season`, enums `MatchType`/`MatchStatus`). No logic, no EF attributes (mapping lives in Infrastructure).
- **DominoTracker.Application** — business logic. `Interfaces/` defines both repository interfaces (`I*Repository`) and service interfaces (`I*Service`); `Services/` implements services against the repository interfaces; `DTOs/` holds request/response shapes returned by controllers; `Exceptions/` holds domain-specific exceptions thrown by services and caught in controllers to map to HTTP status codes.
- **DominoTracker.Infrastructure** — EF Core: `Persistence/AppDbContext.cs` (entity configuration, relationships, cascade rules, seeded `Season` data) and `Repositories/` (repository implementations, each scoped to one entity).
- **DominoTracker.WebAPI** — `Controllers/` (thin, translate service exceptions to HTTP responses), `Program.cs` (DI registration and middleware pipeline).

Note: not every service has a corresponding interface registered in DI (e.g. `TeamService`, `MatchService` are registered as concrete types in `Program.cs`, while `PlayerService`, `WarService`, `SeasonService` are registered against their interfaces). Follow whatever pattern the existing sibling class uses when adding a new service rather than assuming one convention applies everywhere.

### Domain model relationships

- A `Team` is a fixed pair of two `Player`s (`Player1Id`/`Player2Id`), unique per pair.
- A `Match` is between `TeamA` and `TeamB`, has a `MatchType` (`Fixed` or `Rotating`) and a `MatchStatus`, and belongs to a `Season`.
- A `War` is a best-of-N series between the same two teams, made of `WarMatch` join rows (ordered by `Order`) pointing at individual `Match` records. Wars can only be built from already-finished `Fixed` matches (see `WarService.CreateWarAsync`), which validates: all matches exist, are `Fixed`, are finished, share one season, involve the same two teams (regardless of A/B side), and that the exact match-set hasn't already been used for another war (duplicate detection via sorted match-id sequences).
- `Season` rows are fixed, pre-seeded date ranges (via `HasData` in `AppDbContext`) rather than user-created; `Match`/`War` each have a `SeasonId` FK. Several endpoints accept a `seasonIds` query filter, parsed via `SeasonQuery.Parse` (comma-separated ints) in `WebAPI/Controllers/SeasonQuery.cs`.
- Stats services (`TeamStatsService`) compute win/loss/win-rate aggregates by fetching all matches/wars into memory and grouping per team in C#, rather than via SQL aggregation — follow this pattern for new stats endpoints unless performance requires otherwise.

### Conventions worth preserving

- Enums are serialized as strings over the wire (`JsonStringEnumConverter` registered in `Program.cs`).
- Repositories return tracked/no-tracking EF entities with explicit `.Include()` chains for the exact navigation properties each query needs; services map entities to DTOs (never return entities directly from controllers).
- Controllers catch specific typed exceptions from services and map them to specific HTTP responses (e.g. `DuplicateWarException` → 409 Conflict, `NoMatchesForWarException`/`InvalidWarMatchesException` → 400 BadRequest) — follow this pattern rather than generic try/catch-all.
- Some Spanish is used in exception messages and a few comments (the domain is a Spanish-speaking domino group); match existing language per-file rather than converting wholesale.
