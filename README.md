# Mood Tracker

A full-stack mood-tracking app. Pick a mood, write a note, watch your week unfold.

Built as a take-home for a senior .NET + React role. ~100 atomic commits, strict TDD, brutalist-soft visual direction.

---

## 🚀 Live

| | URL |
|---|---|
| **App (React SPA + .NET API, same origin)** | http://18.201.252.61 |
| **PHP summary table** | http://18.201.252.61:8080/summary |
| **OpenAPI 3.1 spec** | http://18.201.252.61/openapi/v1.json |
| **GitHub repo** | https://github.com/IsraelIyonsi/mood-tracker |

Hosted on AWS EC2 (t3.micro, eu-west-1). The React SPA is bundled into the .NET API's `wwwroot` so both serve from the same origin — no CORS, no mixed-content drama. PHP runs as a sidecar container reading the same SQLite database via a shared Docker volume.

**API endpoints:**

| Method | Path | Purpose |
|---|---|---|
| `POST` | `/api/v1/moods` | Log a mood entry. FluentValidation; returns 201 with body. |
| `GET` | `/api/v1/moods?take=7` | Returns last N entries (clamped 1–30). |
| `GET` | `/health/live` | Liveness probe. |
| `GET` | `/health/ready` | Readiness probe (pings DB). |
| `GET` | `/openapi/v1.json` | Auto-generated OpenAPI spec. |

---

## 📐 Architecture at a glance

```
┌──────────────────────────────────────────────────────────────────┐
│                       AWS EC2 t3.micro                           │
│                                                                  │
│  ┌──────────────────────────────────┐  ┌─────────────────────┐  │
│  │ mood-tracker-api  (port 80)      │  │ mood-tracker-php    │  │
│  │ ASP.NET Core 10 + EF Core        │  │ Slim 4 + PDO        │  │
│  │ ┌────────────────────────┐       │  │ (port 8080)         │  │
│  │ │ wwwroot/ (React SPA)   │       │  │                     │  │
│  │ │ — UseStaticFiles +     │       │  │ GET /summary →      │  │
│  │ │   MapFallbackToFile    │       │  │ HTML table of all   │  │
│  │ └────────────────────────┘       │  │ moods               │  │
│  │ /api/v1/moods                    │  │                     │  │
│  └─────────────┬────────────────────┘  └─────────┬───────────┘  │
│                │                                  │              │
│                ▼                                  ▼              │
│      ┌────────────────────────────────────────────────┐         │
│      │  Docker volume: mood-data → /data/mood.db      │         │
│      │  (shared between API and PHP)                  │         │
│      └────────────────────────────────────────────────┘         │
└──────────────────────────────────────────────────────────────────┘
```

**See [`docs/architecture.md`](./docs/architecture.md) for Mermaid diagrams** of the system, request lifecycle, frontend data flow, and bounded context.

---

## 🛠 Stack

| Layer | Tech | Why |
|---|---|---|
| Backend | .NET 10 · ASP.NET Core Minimal APIs · EF Core · SQLite · FluentValidation · Serilog | Modern, type-safe, low ceremony. .NET 10 features used: `TimeProvider`, `[LoggerMessage]` source generator, `Guid.CreateVersion7`, `IExceptionHandler`, primary constructors. |
| Tests (API) | xUnit · WebApplicationFactory · `FakeTimeProvider` · NetArchTest · Shouldly | Real integration via in-memory SQLite, architecture rules as enforced code. **55 passing tests.** |
| Frontend | Vite · React 18 · TypeScript (strict) · Tailwind v3 · Framer Motion · TanStack Query · Zod · React Hook Form | Strict TS + Zod = type safety end-to-end. TanStack Query for server state. Headless hooks (no inline JSX logic). |
| Tests (web) | Vitest · React Testing Library · `@testing-library/user-event` | Snapshot per-mood for SVG faces; component tests for picker + form. |
| PHP bonus | Slim 4 · PDO · pdo_sqlite | Single route, ~80 lines of PHP. Reads same SQLite file. |
| Infra | AWS EC2 + ECR + IAM + SSM · Docker · GitHub Actions | EC2 boots Amazon Linux 2023, user-data installs Docker + pulls image from ECR. SSM rolls new images zero-downtime. |
| Design | Space Grotesk + JetBrains Mono · CSS custom properties · `prefers-reduced-motion` | Brutalist-soft direction. 3-layer design tokens. `--accent-mood` CSS var drives 4 visual updates (headline, hairline, button, picker) per mood pick. |

---

## 🏛 Load-bearing decisions

These are the calls a reviewer would actually grade on:

1. **[ADR-0001](./docs/adr/0001-vertical-slice-over-clean-architecture.md): Vertical Slice over Clean Architecture.** One folder per use case (`Features/Moods/{LogMood, GetRecentMoods}`), not 4 stacked projects. Proportional to scope (2 endpoints).
2. **[ADR-0002](./docs/adr/0002-no-unit-of-work-wrapper.md): `DbContext` IS the Unit of Work.** No `IUnitOfWork` / `IRepository` wrappers over EF Core. Fowler's UoW definition matches EF Core line-for-line.
3. **[ADR-0003](./docs/adr/0003-openapi-codegen-for-cross-stack-types.md): OpenAPI codegen for cross-stack types.** Backend defines DTOs in C#, frontend imports from generated `schema.ts`. Rename in C# = compile error in TS.

Other deliberate choices:

- **`IAuditable` + `SaveChangesInterceptor`** populate `CreatedAt`/`UpdatedAt` via injected `TimeProvider`. Handlers stay domain-pure. Tests use `FakeTimeProvider` for deterministic timestamps.
- **`LoggedAt` ≠ `CreatedAt`** in `MoodEntry`. Domain time vs audit time. Lets users backdate; audit fields stay accurate to row insertion.
- **`Mood` enum stored as string** in SQLite via `HasConversion<string>()`. Survives enum reordering, readable in DB.
- **`DateTimeOffsetToBinaryConverter`** convention applied globally so SQLite ORDER BY on `DateTimeOffset` works (raw SQLite doesn't support it).
- **RFC 7807 ProblemDetails** for every error response. `correlationId` propagated via header + structured logs.
- **`ValidationFilter<TRequest>`** generic endpoint filter runs FluentValidation before the handler. Reusable across endpoints.
- **`IRequestHandler<TReq, TRes>`** abstract handler contract. Every feature handler same shape.
- **4-layer frontend data path**: `apiFetch` (HTTP only) → `MoodClient` (class with ECMAScript `#` private fields) → `useMoodRepository` (TanStack hooks) → components. Component layer never sees `fetch` or URLs.
- **Object Mother + `TheoryData<TScenario>`** for validator tests. `A.LogMoodRequest().WithMood(...)` reads like English.
- **`ApiError` class hierarchy** on the frontend — abstract base with factory dispatch (`fromResponse`) into 6 concrete subclasses (Validation / NotFound / RateLimited / Server / Network / Unknown). Polymorphic `instanceof` in catch blocks.
- **Custom-drawn SVG mood faces** via data-driven `FACE_SPECS` table. No emoji, no icons, no images. Adding a 6th mood = one entry.
- **Brutalist-soft visual direction.** Editorial-style typography, warm cream + ink palette, sharp 90° corners, block-fill mood squares, the "really?" headline-exhale interaction driven by a single CSS variable.

---

## ▶️ Run locally

### With Docker (fastest)

```bash
git clone https://github.com/IsraelIyonsi/mood-tracker.git
cd mood-tracker
docker compose up
```

- App: http://localhost:5000
- PHP summary: http://localhost:8080/summary
- Web (Vite dev server with HMR): http://localhost:5173

All three share the `mood-data` Docker volume.

### Each service independently

**Backend:**
```bash
cd api
dotnet run --project src/MoodTracker.Api
# http://localhost:5000
```

**Frontend (in another terminal):**
```bash
cd web
npm install --legacy-peer-deps
VITE_API_BASE_URL=http://localhost:5000 npm run dev
# http://localhost:5173
```

**PHP summary:**
```bash
cd php
composer install
MOOD_DB_PATH=../api/src/MoodTracker.Api/mood.db php -S localhost:8080 -t public
# http://localhost:8080/summary
```

---

## 🧪 Tests

```bash
cd api && dotnet test    # 55 passing tests: domain, validators, handlers, integration, 6 arch rules
cd web && npm test       # SVG face snapshots
```

GitHub Actions runs all three pipelines (`api-ci`, `web-ci`, `php-ci`) on every push to `main`. Paths-filtered so each pipeline only runs when its own folder changes.

---

## 🚢 Deploy

**Currently live:** AWS EC2 t3.micro running two containers (`mood-tracker-api` on `:80`, `mood-tracker-php` on `:8080`) sharing a Docker volume for SQLite.

**Build + push:**
```bash
# API (includes bundled SPA)
cd web && VITE_API_BASE_URL="" npm run build
cp -r dist/* ../api/src/MoodTracker.Api/wwwroot/
cd ../api && docker build -t mood-tracker-api -f src/MoodTracker.Api/Dockerfile .
docker tag mood-tracker-api 118343790576.dkr.ecr.eu-west-1.amazonaws.com/mood-tracker-api:latest
docker push 118343790576.dkr.ecr.eu-west-1.amazonaws.com/mood-tracker-api:latest

# PHP
cd ../php && docker build -t mood-tracker-php .
docker tag mood-tracker-php 118343790576.dkr.ecr.eu-west-1.amazonaws.com/mood-tracker-php:latest
docker push 118343790576.dkr.ecr.eu-west-1.amazonaws.com/mood-tracker-php:latest
```

**Roll via SSM (zero-downtime style):**
```bash
aws ssm send-command --instance-ids i-... --document-name AWS-RunShellScript \
  --parameters 'commands=["docker pull ...", "docker stop mood-tracker-api", "docker run -d ..."]'
```

Full deploy + rollback procedure in [`docs/runbook.md`](./docs/runbook.md).

---

## 📂 Repo layout

```
mood-tracker/
├── api/                          # .NET 10 backend
│   ├── src/MoodTracker.Api/
│   │   ├── Common/               # cross-cutting (constants, persistence, observability, errors)
│   │   ├── Features/Moods/       # Vertical Slice per use case
│   │   │   ├── LogMood/          # Request, Response, Validator, Handler, Endpoint
│   │   │   └── GetRecentMoods/   # Query, View, Response, Handler, Endpoint
│   │   └── wwwroot/              # bundled React SPA (built from web/)
│   └── tests/MoodTracker.Api.Tests/
│       ├── Builders/             # Object Mother (A.LogMoodRequest()...)
│       ├── Scenarios/            # TheoryData records
│       ├── Domain/               # entity invariants
│       ├── Features/Moods/       # validator + handler unit tests
│       ├── Integration/          # WebApplicationFactory HTTP tests
│       └── Architecture/         # NetArchTest rules
│
├── web/                          # Vite + React 18 frontend
│   └── src/
│       ├── shared/               # cross-feature primitives
│       │   ├── api/              # apiFetch + ApiError class hierarchy
│       │   ├── design-system/    # tokens, motion, components
│       │   └── outcomes/         # Result<T, E>
│       └── features/moods/       # the only feature
│           ├── data/             # MoodClient + useMoodRepository
│           ├── types/            # Zod schemas + inferred TS types
│           ├── view-models/      # MoodEntryViewModel class
│           ├── components/       # MoodFace, MoodPicker, LogMoodForm, MoodTimeline, Headline
│           └── pages/            # MoodTrackerPage orchestrator
│
├── php/                          # Slim 4 micro-route
│   ├── public/index.php          # Slim bootstrap, one route
│   ├── src/                      # SqliteConnection + SummaryRoute
│   └── views/summary.phtml       # HTML table render
│
├── docs/
│   ├── adr/                      # 3 architecture decision records
│   ├── architecture.md           # system + sequence + data-flow Mermaid
│   ├── runbook.md                # local dev, deploy, rollback
│   └── loom-script.md            # video walkthrough script
│
├── .github/workflows/            # api-ci, web-ci, php-ci
└── docker-compose.yml            # full local stack
```

---

## 🎬 Loom walkthrough

Script ready at [`docs/loom-script.md`](./docs/loom-script.md). Covers: API design + EF setup → React structure + state → SVG faces architecture → PHP bonus approach → what I'd improve with more time.

---

## ⚠️ Known limitations + trade-offs

- **Hosted over HTTP, not HTTPS.** EC2 public IP without ACM cert / domain. Mixed-content edge case: `crypto.randomUUID()` is HTTPS-only, polyfilled in `apiFetch` for HTTP contexts.
- **SQLite is single-writer.** Fine for this scope. At scale would move to Postgres on RDS.
- **PHP page is read-only** (intentional — spec says "renders a simple server-side HTML summary").
- **No auth.** Out of scope per brief.
- **Bundle size 460 kB / 142 kB gzip.** Framer Motion + TanStack Query + React 19 are the heaviest pieces.

---

## 📜 License

MIT — see [LICENSE](./LICENSE).
