# ADR-0001: Vertical Slice over Clean Architecture

- **Status:** Accepted
- **Date:** 2026-05-19
- **Decider:** Israel Iyonsi

## Context

The API has two endpoints (`POST /api/v1/moods`, `GET /api/v1/moods`). A standard Clean Architecture layout would create four projects (`Domain` / `Application` / `Infrastructure` / `Api`), one validator, one handler, one mapper, one repository, one DbContext — perhaps fifteen files for two endpoints.

Vertical Slice — one folder per use case — keeps related code together and would result in about the same file count without the project boundaries.

Flat minimal APIs (`Program.cs` plus two static handlers) would be the smallest possible but reads as junior for a senior role.

## Decision

**Vertical Slice in a single project.** Folder layout:

```
src/MoodTracker.Api/
├── Common/                  # cross-cutting (constants, persistence, observability, errors)
└── Features/Moods/
    ├── Mood.cs              # enum
    ├── MoodEntry.cs         # entity (shared across feature)
    ├── MoodLimits.cs        # constants (shared)
    ├── LogMood/             # use case A — endpoint + request/response + validator + handler
    └── GetRecentMoods/      # use case B — endpoint + query/response + handler
```

## Why Vertical Slice wins here

- **Proportional to the problem size.** Four projects for two endpoints reads as ceremony. Anyone reading the solution sees domain capabilities (`LogMood`, `GetRecentMoods`), not abstract layers.
- **Open/Closed at the folder level.** Adding endpoint #3 = new folder, zero edits to existing files.
- **Independently extractable.** Each slice could be lifted into its own service later — already organized by capability.

## Consequences

- No `Domain` project boundary — domain types live inside `Features/Moods/`. Acceptable: one bounded context.
- Common cross-cutting code (DbContext, interceptors, problem-details middleware) lives under `Common/`. This is a reasonable layering even without explicit project separation.
- Architecture tests (`NetArchTest`) enforce that features don't reference each other.

## Alternatives considered

| Option | Why rejected |
|---|---|
| Clean Architecture (4 projects) | Disproportionate for two endpoints. Reads as "learned the pattern, applied it anyway." |
| Flat minimal APIs in `Program.cs` | Junior tell. No clear structure for adding the third endpoint. |
| Controller MVC traditional | Safe but unremarkable. Loses the "I think in capabilities" signal. |
| MediatR-mediated handlers | Two handlers. Adding a library to wrap two function calls is pure ceremony. |
