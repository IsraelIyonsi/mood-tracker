# Loom Walkthrough Script (≤5 min)

Target audience: senior .NET engineer reviewing a take-home. Keep it dense, no apologising, no fluff.

## Open (0:00 – 0:20)

> "This is a full-stack mood tracker. .NET 10 backend, Vite + React 18 frontend, PHP Slim bonus, all in one monorepo. Strict TDD throughout, RFC 7807 errors, custom-drawn SVG faces, OpenAPI-generated TypeScript DTOs across the stack. Source: `github.com/IsraelIyonsi/mood-tracker`. ~75 atomic commits over ~24 hours."

Show the GitHub repo briefly.

## API design + EF setup (0:20 – 1:30)

Open `api/src/MoodTracker.Api/Features/Moods/LogMood/`.

> "Vertical Slice — one folder per use case. ADR-0001 explains why over Clean Architecture for two endpoints. Each slice has Request, Response, Validator, Handler, Endpoint. Handler implements `IRequestHandler<TReq, TRes>` — generic abstraction, same shape for every feature."

Open `MoodEntry.cs`.

> "Entity is immutable — `private init` setters. Implements `IAuditable` — `CreatedAt`, optional `UpdatedAt`. Crucially, `LoggedAt` ≠ `CreatedAt`: LoggedAt is when the user *felt* it, CreatedAt is when the row was inserted. Domain time vs audit time."

Open `AuditInterceptor.cs`.

> "Audit fields are populated by an EF `SaveChangesInterceptor`, not by the handler. The handler stays domain-pure. Clock injected via `TimeProvider` — .NET 8 idiom, fully testable with `FakeTimeProvider`. ADR-0002 explains why there's no `IUnitOfWork` wrapper — `DbContext` IS the unit of work."

Open `LogMoodValidator.cs`.

> "FluentValidation. Note max length, mood range, time bounds — all read from `IOptions<MoodOptions>` bound to `appsettings.json`. Zero magic numbers in code."

Open `LogMoodEndpoint.cs`.

> "Minimal API at the edge, generic `ValidationFilter<TRequest>` runs before the handler. RFC 7807 ProblemDetails on every error path. Rate limiter on POST. Correlation ID middleware on every request."

## React structure + state (1:30 – 2:30)

Open `web/src/features/moods/`.

> "Same Vertical Slice mirror on the frontend. Feature folder by capability."

Open `data/MoodClient.ts`.

> "Four-layer data path. `apiFetch` is the only thing that touches `fetch` — single seam. `MoodClient` is a class with ECMAScript private fields — parses inbound JSON with Zod, so the boundary is symmetric: backend validates with FluentValidation, frontend validates with Zod. Component layer never imports `fetch` or URLs."

Open `useMoodRepository.ts`.

> "TanStack Query wraps the client. `useRecentMoods`, `useLogMood`. Component layer consumes these hooks only."

Open `LogMoodForm/useLogMoodForm.ts` + `LogMoodForm.tsx`.

> "Headless hook owns ALL state. Component is pure JSX — props in, JSX out. No inline arrow functions, no ternaries in the markup. The view is declarative. Hook returns `mood`, `setMood`, `onSubmit`, `canSubmit`, `submitLabel` — derived values pre-computed."

Open `view-models/MoodEntryViewModel.ts`.

> "Each timeline card uses a `MoodEntryViewModel` — class with getters for derived presentation properties. Constructed inside `useMemo` per entry. Crucially, the DTO stays plain in the TanStack cache — the VM lives only in render. Crossing that line breaks React Query's reference equality."

## SVG-drawn mood faces (2:30 – 3:30)

Open `components/MoodFace/faceSpecs.ts`.

> "Brief mandated drawing the faces with primitives — no emoji, no icons. Data-driven: `FACE_SPECS` is a table from `Mood` to a `FaceSpec` — eyebrows, eyes, mouth. Each variant is just a spec. Adding a sixth mood is one entry."

Open `MoodFace.tsx`.

> "One component reads the spec and renders sub-primitives: `<Eyebrow>`, `<Eye>`, `<Mouth>`. Mouth has four shape types — arc path, line, ellipse, rectangle. All drawn with square stroke-linecap because the visual direction is brutalist-soft."

Show the live app — click through all 5 moods.

> "Brutalist visual direction. Editorial paper background, Space Grotesk display, JetBrains Mono labels. The headline interaction: click any mood, and the word 'really?' recolors plus a thick underline draws. Single CSS custom property — `--accent-mood` — driven by the picker. Submit button morphs, mood squares invert to block-fill with the cream face inside. One variable, four visual changes. The 5 timeline cards are full mood-color blocks with a cream-bordered face well — click any one and it triggers a brutalist swell animation."

## PHP bonus (3:30 – 4:00)

Open `php/src/SummaryRoute.php`.

> "Slim 4 micro-route. Opens the same SQLite database the .NET API writes to. PDO. Returns a server-rendered HTML table grouped by mood. Runs locally via `docker compose up php` — shares the `/data` volume with the API container. Not deployed — included as a runnable bonus."

## What I'd improve with more time (4:00 – 4:45)

Pick 3-4, don't list everything.

- "Real-time updates via SignalR or SSE — new mood entries would push to the timeline without manual refresh."
- "Mood streaks and weekly trend analytics — would land as a `Features/Stats/` slice with its own `MoodWeek` calculator class."
- "Service Worker for offline-first logging — write to IndexedDB when offline, sync to API when back online."
- "OpenTelemetry distributed traces — currently I'm doing structured Serilog with correlation IDs, which is good for single-service but I'd want OTel across the React → API → DB path if this went into a mesh."
- "Idempotency-Key header on POST — for mood data the cost of a dupe is near zero so I deliberately skipped it, but for a payments service I'd add it day one. Knowing when not to apply the pattern is part of the design."

## Close (4:45 – 5:00)

> "Live web app at `<VERCEL_URL>`. API at `<RAILWAY_URL>`. All 60+ tests passing in CI, including architecture rules. Thanks for the brief — happy to walk through any of this in more detail."

---

## Recording tips

- Use Loom's screen + camera mode. The face-on view matters for a take-home.
- Highlight specific lines as you talk — Loom has a click-highlight feature.
- Record in 3-4 takes max, splice in Loom's editor.
- Aim for 4:30, ship at 5:00 hard cap.
- Skip the apologies. "I'd improve X" is fine. "Sorry I didn't have time for X" is not.
