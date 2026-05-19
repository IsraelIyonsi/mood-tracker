# ADR-0002: DbContext IS the Unit of Work — no `IUnitOfWork` wrapper

- **Status:** Accepted
- **Date:** 2026-05-19
- **Decider:** Israel Iyonsi

## Context

A common pattern in .NET codebases is to wrap EF Core's `DbContext` in `IUnitOfWork` and `IRepository<T>` abstractions, on the assumption that this aids testability or decoupling.

## Decision

**Do not wrap `DbContext` in `IUnitOfWork` or `IRepository<T>`.** Handlers take `MoodDbContext` directly via DI. Each handler calls `SaveChangesAsync(ct)` once.

## Why

Compare Fowler's Unit of Work definition with what EF Core's `DbContext` already does:

| Fowler's UoW | DbContext |
|---|---|
| Tracks changes to entities during a business transaction | `ChangeTracker` |
| Coordinates writes across multiple repositories | `DbSet<T>` per entity |
| Commits all changes in one transaction | `SaveChangesAsync()` |
| Maintains identity map | First-level cache by key |
| Rollback on failure | Implicit transaction scope |

EF Core implements Fowler's pattern. Wrapping it in `IUnitOfWork` is wrapping an abstraction in another abstraction.

A custom repository over EF Core typically:
- Returns `IEnumerable<T>` instead of `IQueryable<T>` — losing projection, eager loading composition, `AsSplitQuery`, etc.
- Adds four files per entity (interface + impl for repo, interface + impl for UoW).
- Forces tests to mock the wrapper rather than use the in-memory or SQLite provider directly.
- Adds no real safety — handlers still depend on EF semantics.

> "If you wrap EF in a Repository, you're testing your wrapper, not your code." — paraphrasing Greg Young

## When `IUnitOfWork` would be justified

Three cases (none apply to this app):

1. **Multi-database transactions** — a single business operation must atomically write to two different databases (or DB + cache + message bus). The UoW coordinates the saga / outbox.
2. **Strict DDD** — the `Domain` project must not reference EF Core. An `IUnitOfWork` interface lives in `Domain`, implemented in `Infrastructure`.
3. **MediatR pipeline behavior** — a `TransactionalBehavior<TReq, TRes>` auto-commits after every command handler runs, removing `SaveChangesAsync` boilerplate from every handler.

This app has none of these conditions.

## Consequences

- Handlers inject `MoodDbContext` directly. One `SaveChangesAsync(ct)` per handler = one atomic transaction.
- Explicit `BeginTransactionAsync` is used only when a handler needs multiple `SaveChangesAsync` calls to be atomic together. None do, currently.
- Audit fields (`CreatedAt`, `UpdatedAt`) are populated by `AuditInterceptor : SaveChangesInterceptor`, not by handlers — keeps domain code pure.
- The same decision is mentioned in the Loom video as the senior signal: knowing when *not* to apply the pattern.
