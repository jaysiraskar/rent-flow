# RentFlow

RentFlow is an MVP micro SaaS for small landlords and PG owners in India.

## Backend status
Implemented backend API for:
- JWT auth (register/login/me)
- Property CRUD
- Tenant CRUD
- Monthly rent record generation and payment updates
- Dashboard summary + upcoming dues + overdue
- Background rent reminder processing (email channel) + reminder logs
- EF Core SQL Server context, migrations, and seed data

## Seed credentials
- Email: `demo@rentflow.in`
- Password: `Demo@123`

## API base
- `/api/v1`

## Quick run (backend, local dotnet)
1. Configure connection string in `backend/src/RentFlow.Api/appsettings.json`.
2. Run API from `backend/src/RentFlow.Api`:
   - `dotnet restore`
   - `dotnet ef database update --project ../RentFlow.Infrastructure --startup-project .`
   - `dotnet run`

## Quick run (full stack with Docker)
1. Ensure Docker Desktop (or Docker Engine + Compose plugin) is running.
2. From repo root, start all services:
   - `docker compose up --build`
3. Open apps:
   - Frontend: `http://localhost:4200`
   - API: `http://localhost:8080`
   - API health: `http://localhost:8080/health`
4. Stop services:
   - `docker compose down`
5. To remove DB data volume as well:
   - `docker compose down -v`

## Operational notes
- Reminder job run summary includes checked/sent/failed counts and processing timestamp.
- Reminder logs can be filtered by year/month/property in API and frontend.
- Default compose SQL password is for local development only; change before sharing/deploying.

## Logging guidance
- API logs are configured via ASP.NET logging levels in `appsettings*.json`.
- For debugging reminder dispatches, check API logs + `/api/v1/reminders/logs`.

## Notes
- Frontend includes auth, dashboard, properties, tenants, rent records, and reminder logs views.
- Background reminder job is implemented for email reminders and logs each reminder attempt.
- WhatsApp can be added later by implementing a new `INotificationChannel`.
