# PHP Summary Page

A Slim 4 micro-route that reads the same SQLite database as the .NET API and renders an HTML summary table.

Optional bonus from the test task spec. Local-only — not deployed.

## Run locally

```bash
docker compose up php
```

Then open http://localhost:8080/summary.

The PHP container mounts the same `/data` volume as the API, so both processes read the same `mood.db` file.

## Manual (no Docker)

```bash
composer install
MOOD_DB_PATH=/path/to/mood.db php -S localhost:8080 -t public
```
