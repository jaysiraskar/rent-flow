# RentFlow

RentFlow is an MVP micro SaaS for small landlords and PG owners in India.

## Backend status
Implemented backend API for:
- JWT auth (register/login)
- Property CRUD
- Tenant CRUD
- Monthly rent record generation and payment updates
- Dashboard summary
- EF Core SQL Server context, initial migration, and seed data

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
- Frontend implementation is intentionally pending.
- Reminder background jobs are pending for the next phase.
