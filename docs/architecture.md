# Architecture

## System overview

```mermaid
flowchart LR
    subgraph Browser["Browser"]
        UI["React SPA<br/>(Vite + Tailwind + Framer)"]
    end

    subgraph Vercel["Vercel CDN"]
        Web["Static bundle"]
    end

    subgraph Railway["Railway"]
        API["ASP.NET Core 10<br/>Minimal APIs"]
        Vol[("Persistent volume<br/>/data/mood.db")]
        API -->|EF Core| Vol
    end

    subgraph LocalOnly["Local-only (Docker)"]
        PHP["Slim 4 PHP<br/>/summary"]
        SharedDB[("Shared SQLite<br/>via Docker volume")]
        PHP -->|PDO| SharedDB
        API_Local["API container"] -->|EF Core| SharedDB
    end

    UI -->|fetch + Zod parse| Web
    Web -.->|VITE_API_BASE_URL| API
    API -->|OpenAPI spec| GenScript["openapi-typescript<br/>(CI step)"]
    GenScript -->|schema.ts| UI
```

## Request lifecycle (POST /api/v1/moods)

```mermaid
sequenceDiagram
    autonumber
    participant U as User
    participant W as React SPA
    participant C as MoodClient (class)
    participant F as apiFetch
    participant E as LogMoodEndpoint
    participant V as ValidationFilter
    participant H as LogMoodHandler
    participant I as AuditInterceptor
    participant D as MoodDbContext + SQLite

    U->>W: clicks "commit happy entry →"
    W->>C: useLog().mutate(request)
    C->>C: LogMoodRequestSchema.parse(request)
    C->>F: POST /api/v1/moods + Correlation-Id
    F->>E: HTTP request
    E->>V: ValidationFilter<LogMoodRequest>
    V->>V: FluentValidation rules
    alt invalid
        V-->>F: 422 ProblemDetails
        F-->>C: throws ValidationApiError
        C-->>W: surfaces field errors
    else valid
        V->>H: HandleAsync(request, ct)
        H->>D: db.MoodEntries.Add(entry)
        H->>D: SaveChangesAsync(ct)
        D->>I: SavingChanges hook
        I->>D: sets CreatedAt via TimeProvider
        D-->>H: persisted
        H-->>E: LogMoodResponse.From(entry)
        E-->>F: 201 Created + Location + body
        F->>F: response.json() + Zod parse
        F-->>C: LogMoodResponse
        C-->>W: invalidates query, timeline refreshes
    end
```

## Frontend data flow

```mermaid
flowchart TB
    subgraph Components["Component layer"]
        Form["LogMoodForm.tsx<br/>(declarative JSX only)"]
        Timeline["MoodTimeline.tsx"]
    end

    subgraph Hooks["Headless hook layer"]
        UseForm["useLogMoodForm.ts<br/>(state + handlers)"]
        UseRepo["useMoodRepository.tsx<br/>(TanStack Query wrappers)"]
        UseVm["useTimelineCard.ts<br/>(viewmodel construction)"]
    end

    subgraph Data["Domain client layer"]
        Client["MoodClient (class)<br/>private #fetch, public log() + getRecent()"]
        VM["MoodEntryViewModel (class)<br/>derived props as getters"]
    end

    subgraph Primitive["HTTP primitive"]
        Fetch["apiFetch&lt;T&gt;<br/>(the ONLY fetch in the app)"]
    end

    Form --> UseForm
    Timeline --> UseRepo
    Timeline --> UseVm
    UseForm --> UseRepo
    UseRepo --> Client
    UseVm --> VM
    Client --> Fetch
    Fetch -->|HTTP| API[(API)]

    classDef forbidden stroke:#F43F5E,stroke-width:2px
    Form:::forbidden
    Timeline:::forbidden
```

> **Rule:** the component layer never imports `fetch`, `MoodClient`, or query keys. It only consumes hooks. Enforced by lint rule + code review.

## Bounded context

```mermaid
flowchart LR
    subgraph Moods["Features/Moods (bounded context)"]
        Mood["Mood enum"]
        MoodEntry["MoodEntry entity"]
        LogMood["LogMood/"]
        GetRecent["GetRecentMoods/"]
        Limits["MoodLimits"]
    end

    LogMood --> Mood
    LogMood --> MoodEntry
    LogMood --> Limits
    GetRecent --> Mood
    GetRecent --> MoodEntry
    GetRecent --> Limits

    Future["Future features:<br/>Journal/, Streaks/, Stats/"] -.-> Moods
```

Architecture tests enforce that future features cannot reach into `Features/Moods/` internals — they get a `MoodEntryView` DTO via a public contract.

## See also

- [ADR-0001 — Vertical Slice over Clean Architecture](./adr/0001-vertical-slice-over-clean-architecture.md)
- [ADR-0002 — DbContext IS the Unit of Work](./adr/0002-no-unit-of-work-wrapper.md)
- [ADR-0003 — OpenAPI codegen for cross-stack types](./adr/0003-openapi-codegen-for-cross-stack-types.md)
