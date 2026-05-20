# Mood Tracker

A full-stack mood-tracking app. Pick a mood, write a note, watch your week unfold.

Built as a take-home for a senior .NET + React role.

## Stack

| Layer | Tech |
|---|---|
| API | .NET 10 · ASP.NET Core Minimal APIs · EF Core · SQLite · FluentValidation · Serilog · xUnit + WebApplicationFactory |
| Web | Vite · React 18 · TypeScript (strict) · Tailwind v3 · Framer Motion · TanStack Query · Zod · React Hook Form · MSW · Vitest · Playwright |
| PHP bonus | Slim 4 · PDO · SQLite (shared with API) |
| Deploy | AWS EC2 (single-host serving SPA + API) · ECR image registry · PHP local-only |

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

Full local stack via Docker (API + Web + PHP share the SQLite volume):

```bash
docker compose up
```

Or run each independently — see [`docs/runbook.md`](./docs/runbook.md).

## Live

**App + API:** http://18.201.252.61 (AWS EC2 t3.micro, single-origin host serving the React SPA and the .NET API)

| Endpoint | Method | Purpose |
|---|---|---|
| `/` | GET | React SPA (Brutalist-soft UI) |
| `/api/v1/moods` | POST | Log a mood entry |
| `/api/v1/moods?take=7` | GET | Last 7 entries (max 30 via clamp) |
| `/health/live` | GET | Liveness probe |
| `/health/ready` | GET | Readiness (DB ping) |
| `/openapi/v1.json` | GET | OpenAPI 3.1 spec |

PHP summary page is local-only (`docker compose up php` then `http://localhost:8080/summary`).

## Tests

```bash
cd api && dotnet test       # 55+ tests: domain, validators, handlers, integration, architecture
cd web && npm test          # SVG face snapshots + component + hook
```

CI runs all three pipelines on every push to `main`.

## Repo layout

```
mood-tracker/
├── api/        # .NET 10 backend (src/ + tests/)
├── web/        # Vite + React frontend
├── php/        # Slim micro-route (local-only)
├── docs/
│   ├── adr/                # architecture decision records
│   ├── architecture.md     # system + sequence + data-flow diagrams
│   ├── runbook.md          # local dev, deploy, rollback
│   └── loom-script.md      # video walkthrough script
└── docker-compose.yml
```

## License

MIT — see [LICENSE](./LICENSE).
