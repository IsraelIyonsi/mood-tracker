# ADR-0003: OpenAPI codegen for cross-stack type safety

- **Status:** Accepted
- **Date:** 2026-05-19
- **Decider:** Israel Iyonsi

## Context

The backend and frontend live in the same monorepo but separate languages (C# / TypeScript). Without coordination, DTOs drift: a backend rename (`note` → `text`) silently breaks the frontend at runtime.

Three options:

1. Hand-mirror DTOs — copy the C# shape to TS by hand
2. Share types via a monorepo package — both consume from a shared module
3. Generate TS types from the backend's OpenAPI spec

## Decision

**Option 3: generate TypeScript types from the OpenAPI spec produced by the API.**

The API exposes `/openapi/v1.json`. A frontend script regenerates `web/src/shared/api/schema.ts` from it. CI runs this regeneration and fails the build if `schema.ts` diverges from what the API exposes.

```bash
# web/package.json
"types:gen": "openapi-typescript http://localhost:5000/openapi/v1.json -o src/shared/api/schema.ts"
```

The frontend imports DTOs from the generated module:

```ts
import type { components } from '@/shared/api/schema';
type LogMoodRequest = components['schemas']['LogMoodRequest'];
type MoodEntry      = components['schemas']['MoodEntry'];
```

Zod schemas validate inbound JSON at runtime; the generated types ensure compile-time correctness against the API contract.

## Why

- **Zero hand-mirroring.** No risk of forgetting to update the TS DTO when the C# DTO changes.
- **Single source of truth.** The C# DTO is the spec. Everything downstream derives.
- **Refactor safety.** Backend renames cascade to frontend compile errors at CI time.

## Why not the alternatives

**Hand-mirroring** is fastest to start, but the moment one DTO changes silently and isn't ported, a runtime bug ships.

**Shared types package** in a monorepo requires either a separate publish step or a TS workspace that the C# project can't consume. Either way, the C# project still has to be the source — at which point, generation is the natural mechanism.

## Consequences

- `web/src/shared/api/schema.ts` is generated, not hand-edited. Added to `.gitignore`. Regenerated as part of CI's `types:check` step.
- Zod schemas still exist (parsing untrusted JSON at runtime) but their inferred TS types may diverge from the generated ones. Use the generated types for compile-time correctness; use Zod for runtime parsing.
- Local dev workflow: backend must be running on port 5000 for `npm run types:gen` to succeed. CI uses a built artifact path instead.
