# Mood Tracker

A small, focused journaling app for tracking how you feel over time. Pick a mood, write a line if you want, and watch your week build itself out.

---

## 🚀 Live

| | URL |
|---|---|
| **App (React SPA + .NET API)** | http://18.201.252.61 |
| **Server-rendered summary (PHP)** | http://18.201.252.61:8080/summary |
| **OpenAPI 3.1 spec** | http://18.201.252.61/openapi/v1.json |
| **Walkthrough video** | (https://www.loom.com/share/81f58a430b4542a2a8ac6bd1a801c907) |

The React SPA is bundled into the .NET API's `wwwroot` so both serve from the same origin — no CORS, no mixed-content. PHP runs as a sidecar container reading the same SQLite database via a shared Docker volume.

### API endpoints

| Method | Path | Purpose |
|---|---|---|
| `POST` | `/api/v1/moods` | Log a mood entry. Validated via FluentValidation; returns 201 with body. |
| `GET` | `/api/v1/moods?take=7` | Returns last N entries (clamped 1–30). |
| `GET` | `/health/live` | Liveness probe. |
| `GET` | `/health/ready` | Readiness probe (pings the DB). |
| `GET` | `/openapi/v1.json` | Auto-generated OpenAPI spec. |

---

## 📐 Architecture

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

See [`docs/architecture.md`](./docs/architecture.md) for Mermaid diagrams of the system, request lifecycle, frontend data flow, and bounded context.

---

## 🛠 Stack

| Layer | Tech |
|---|---|
| **API** | .NET 10 · ASP.NET Core Minimal APIs · EF Core · SQLite · FluentValidation · Serilog |
| **API tests** | xUnit · WebApplicationFactory · `FakeTimeProvider` · NetArchTest · Shouldly (~55 tests) |
| **Web** | Vite · React 18 · TypeScript strict · Tailwind v3 · Framer Motion · TanStack Query · Zod · React Hook Form |
| **Web tests** | Vitest · React Testing Library |
| **PHP** | Slim 4 · PDO · pdo_sqlite |
| **Infra** | AWS EC2 · ECR · IAM · SSM · Docker · GitHub Actions |
| **Design** | Space Grotesk + JetBrains Mono · CSS custom properties · `prefers-reduced-motion` honoured |

---

## 🏛 Key decisions

Three documented:

1. **[ADR-0001](./docs/adr/0001-vertical-slice-over-clean-architecture.md): Vertical Slice over Clean Architecture.** One folder per use case (`Features/Moods/{LogMood, GetRecentMoods}`).
2. **[ADR-0002](./docs/adr/0002-no-unit-of-work-wrapper.md): `DbContext` IS the Unit of Work.** No `IUnitOfWork` / `IRepository` wrappers over EF Core.
3. **[ADR-0003](./docs/adr/0003-openapi-codegen-for-cross-stack-types.md): OpenAPI codegen for cross-stack types.** Backend defines DTOs in C#, frontend imports from a generated `schema.ts`.

Other deliberate choices:

- **`IAuditable` + `SaveChangesInterceptor`** populate `CreatedAt`/`UpdatedAt` via injected `TimeProvider`. Handlers stay domain-pure; tests use `FakeTimeProvider`.
- **`LoggedAt` ≠ `CreatedAt`** in `MoodEntry`. Domain time vs audit time. Lets users backdate; audit fields stay accurate to row insertion.
- **`Mood` enum stored as string** in SQLite via `HasConversion<string>()`. Survives enum reordering, readable in DB.
- **`DateTimeOffsetToBinaryConverter`** convention applied globally so SQLite `ORDER BY` on `DateTimeOffset` works.
- **RFC 7807 ProblemDetails** for every error response. `correlationId` propagated via header + structured logs.
- **`ValidationFilter<TRequest>`** generic endpoint filter runs FluentValidation before the handler. Reusable across endpoints.
- **`IRequestHandler<TReq, TRes>`** abstract handler contract — every feature handler has the same shape.
- **Four-layer frontend data path**: `apiFetch` (HTTP only) → `MoodClient` (class with ECMAScript `#` private fields) → `useMoodRepository` (TanStack hooks) → components. Components never see `fetch` or URLs.
- **Object Mother + `TheoryData<TScenario>`** for validator tests. `A.LogMoodRequest().WithMood(...)` reads like English.
- **`ApiError` class hierarchy** on the frontend — abstract base with factory dispatch (`fromResponse`) into 6 concrete subclasses (Validation / NotFound / RateLimited / Server / Network / Unknown). Polymorphic `instanceof` in catch blocks.
- **Custom-drawn SVG mood faces** via a data-driven `FACE_SPECS` table. No emoji, no icons, no images. Adding a sixth mood is a one-line change.
- **Brutalist-soft visual direction.** Editorial typography, warm cream + ink palette, sharp 90° corners, block-fill mood squares, the "really?" headline-exhale interaction driven by a single CSS variable.

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

**Backend**
```bash
cd api
dotnet run --project src/MoodTracker.Api
# http://localhost:5000
```

**Frontend** (in another terminal)
```bash
cd web
npm install --legacy-peer-deps
VITE_API_BASE_URL=http://localhost:5000 npm run dev
# http://localhost:5173
```

**PHP summary**
```bash
cd php
composer install
MOOD_DB_PATH=../api/src/MoodTracker.Api/mood.db php -S localhost:8080 -t public
# http://localhost:8080/summary
```

---

## 🧪 Tests

```bash
cd api && dotnet test    # domain, validators, handlers, integration, 6 architecture rules
cd web && npm test       # SVG face snapshots + component tests
```

GitHub Actions runs all three pipelines (`api-ci`, `web-ci`, `php-ci`) on every push to `main`. Paths-filtered so each pipeline only runs when its own folder changes.

---

## 🚢 Deploy

Two containers on EC2 sharing a Docker volume for SQLite.

**Build + push:**
```bash
# API (includes bundled SPA)
cd web && VITE_API_BASE_URL="" npm run build
cp -r dist/* ../api/src/MoodTracker.Api/wwwroot/
cd ../api && docker build -t mood-tracker-api -f src/MoodTracker.Api/Dockerfile .
docker tag mood-tracker-api <ecr-repo>/mood-tracker-api:latest
docker push <ecr-repo>/mood-tracker-api:latest

# PHP
cd ../php && docker build -t mood-tracker-php .
docker tag mood-tracker-php <ecr-repo>/mood-tracker-php:latest
docker push <ecr-repo>/mood-tracker-php:latest
```

**Zero-downtime roll** via SSM:
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
│       └── features/moods/
│           ├── data/             # MoodClient + useMoodRepository
│           ├── types/            # Zod schemas + inferred TS types
│           ├── view-models/      # MoodEntryViewModel class
│           ├── components/       # MoodFace, MoodPicker, LogMoodForm, MoodTimeline, Headline
│           └── pages/            # MoodTrackerPage orchestrator
│
├── php/                          # Slim 4 micro-route
│   ├── public/index.php          # Slim bootstrap
│   ├── src/                      # SqliteConnection + SummaryRoute
│   └── views/summary.phtml       # HTML table
│
├── docs/
│   ├── adr/                      # 3 architecture decision records
│   ├── architecture.md           # system + sequence + data-flow Mermaid
│   └── runbook.md                # local dev, deploy, rollback
│
├── .github/workflows/            # api-ci, web-ci, php-ci
└── docker-compose.yml            # full local stack
```

---

## 🗺 Roadmap / known limitations

- **Hosted over HTTP.** No domain or TLS cert yet. `crypto.randomUUID()` is HTTPS-only, polyfilled in `apiFetch` for this reason.
- **SQLite is single-writer.** Fine for current scale. Move to Postgres if write volume grows.
- **PHP page is read-only** by design.
- **No auth.** All entries are anonymous. Adding auth would tie `MoodEntry.UserId` to the authenticated principal.

---

## 📜 License

MIT — see [LICENSE](./LICENSE).
