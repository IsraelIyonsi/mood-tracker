# Mood Tracker

A full-stack mood-tracking app. Pick a mood, write a note, watch your week unfold.

Built as a take-home for a senior .NET + React role.

## Stack

| Layer | Tech |
|---|---|
| API | .NET 10 · ASP.NET Core Minimal APIs · EF Core · SQLite · FluentValidation · Serilog · xUnit + WebApplicationFactory |
| Web | Vite · React 18 · TypeScript (strict) · Tailwind v3 · Framer Motion · TanStack Query · Zod · React Hook Form · MSW · Vitest · Playwright |
| PHP bonus | Slim 4 · PDO · SQLite (shared with API) |
| Deploy | Vercel (web) · Railway (API + PHP local-only) |

## Architecture (TL;DR)

- **Vertical Slice** in the API — one folder per use case, no Clean-Architecture layer-cake
- **`DbContext` IS the Unit of Work** — no `IUnitOfWork`/`IRepository` wrappers over EF Core
- **`IAuditable` + `SaveChangesInterceptor`** for `CreatedAt` / `UpdatedAt`, populated via injected `TimeProvider`
- **RFC 7807 ProblemDetails** for all error responses
- **OpenAPI codegen** generates TypeScript DTOs in the frontend — backend rename = frontend compile error
- **4-layer frontend data path**: `apiFetch` → `MoodClient` (class) → `useMoodRepository` (hook) → components
- **Custom-drawn SVG mood faces** via a data-driven `FACE_SPECS` table — no emoji, no icons
- **Strict TDD** throughout (`test (red)` → `feat`)

See [`docs/adr/`](./docs/adr/) for the load-bearing decisions.

## Run

```bash
# Full local stack via Docker (API + Web + PHP all share the SQLite volume)
docker-compose up

# Or run each separately:

# API
cd api && dotnet run --project src/MoodTracker.Api

# Web
cd web && npm install && npm run dev

# PHP summary page (local only)
cd php && composer install && php -S localhost:8080 -t public
```

## Live

- Web: _(TBD — Vercel)_
- API: _(TBD — Railway)_
- PHP summary: local only — `docker-compose up` then `http://localhost:8080/summary`

## Repo layout

```
mood-tracker/
├── api/        # .NET 10 backend (src/ + tests/)
├── web/        # Vite + React frontend
├── php/        # Slim micro-route
├── docs/       # ADRs + architecture diagrams
└── docker-compose.yml
```

## License

MIT — see [LICENSE](./LICENSE).
