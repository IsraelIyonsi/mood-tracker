# Runbook

## Local development

### Full stack via Docker

```bash
docker compose up
```

- API: http://localhost:5000 (OpenAPI spec at `/openapi/v1.json`, health at `/health/live`)
- Web: http://localhost:5173
- PHP summary: http://localhost:8080/summary

All three share the SQLite volume `mood-data`.

### Run each service independently

**API:**
```bash
cd api
dotnet run --project src/MoodTracker.Api
```

**Web:**
```bash
cd web
npm install --legacy-peer-deps
npm run dev
```

**PHP:**
```bash
cd php
composer install
MOOD_DB_PATH=/path/to/mood.db php -S localhost:8080 -t public
```

## Tests

**API:**
```bash
cd api
dotnet test
```
Includes unit tests, integration tests via `WebApplicationFactory`, and `NetArchTest` architecture rules.

**Web:**
```bash
cd web
npm test
```

## Deployment

### API → Railway

1. `gh repo` is already public; Railway connects via GitHub.
2. From Railway dashboard: New Project → Deploy from GitHub repo → Select `IsraelIyonsi/mood-tracker`.
3. Set root path to `api/`. Railway picks up `railway.toml` and uses `src/MoodTracker.Api/Dockerfile`.
4. Add a persistent volume mounted at `/data` (the `Database__ConnectionString` env var in the Dockerfile points there).
5. Optional env overrides:
   - `Cors__AllowedOrigins__0=https://<vercel-url>`
   - `ASPNETCORE_ENVIRONMENT=Production`

### Web → Vercel

1. From Vercel dashboard: New Project → Import `IsraelIyonsi/mood-tracker`.
2. Set root directory to `web/`. Vercel picks up `vercel.json`.
3. Environment variable: `VITE_API_BASE_URL=https://<railway-url>`.
4. Deploy. Subsequent pushes to `main` auto-deploy.

### PHP

Not deployed. Local-only via Docker.

## Rollback

**Railway:** Dashboard → Deployments → click previous deployment → Redeploy.

**Vercel:** Dashboard → Deployments → click previous deployment → Promote to Production.

## Common issues

- **CORS preflight 401/403**: confirm the deployed Vercel domain is in `Cors__AllowedOrigins` on Railway.
- **SQLite reset after Railway redeploy**: confirm the persistent volume is still mounted at `/data`.
- **Frontend white screen, no errors**: check `VITE_API_BASE_URL` is set and points to the deployed API.
