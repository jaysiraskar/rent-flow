# RentFlow

RentFlow is an MVP micro SaaS for small landlords and PG owners in India.

## Backend status
Implemented backend API for:
- JWT auth (register/login)
- Property CRUD
- Tenant CRUD
- Monthly rent record generation and payment updates
- Dashboard summary
- Background rent reminder processing (email channel) + reminder logs
- EF Core SQL Server context, migrations, and seed data

## Seed credentials
- Email: `demo@rentflow.in`
- Password: `Demo@123`

## API base
- `/api/v1`

## Quick run (backend)
1. Configure connection string in `backend/src/RentFlow.Api/appsettings.json`.
2. Run API from `backend/src/RentFlow.Api`:
   - `dotnet restore`
   - `dotnet ef database update --project ../RentFlow.Infrastructure --startup-project .`
   - `dotnet run`

## Notes
- Frontend includes auth, dashboard, properties, tenants, rent records, and reminder logs views.
- Background reminder job is implemented for email reminders and logs each reminder attempt.
- WhatsApp can be added later by implementing a new `INotificationChannel`.
