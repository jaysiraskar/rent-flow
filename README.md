# RentFlow

RentFlow is an MVP rent management app for small landlords/PG owners.

## Prerequisites
- Docker + Docker Compose
- (Optional local dev) .NET 8 SDK, Node 22+

## 1) Quick start (Docker - local)
```bash
cp .env.example .env
# update .env values (especially MSSQL_SA_PASSWORD and JWT_KEY)
docker compose up --build -d
```

### URLs
- Frontend: `http://localhost:4200`
- API: `http://localhost:8080`
- SQL Server: `localhost:1433`

### Default seeded login
- Email: `demo@rentflow.in`
- Password: `Demo@123`

## 2) Environment variables
All runtime config is provided via `.env` and injected into containers:
- DB: `MSSQL_SA_PASSWORD`
- JWT: `JWT_KEY`, `JWT_ISSUER`, `JWT_AUDIENCE`
- SMTP: `SMTP_HOST`, `SMTP_PORT`, `SMTP_USERNAME`, `SMTP_PASSWORD`, `SMTP_FROM_EMAIL`, `SMTP_FROM_NAME`, `SMTP_ENABLE_SSL`
- Reminder job: `REMINDER_UPCOMING_DAYS`, `REMINDER_INTERVAL_MINUTES`

Use `.env.example` as template.

## 3) Server deployment (single VM)
1. Install Docker + Compose on server.
2. Copy project to server.
3. Create `.env` with production secrets.
4. Run:
   ```bash
   docker compose up --build -d
   ```
5. Put Nginx/Caddy in front (TLS + domain) and proxy:
   - `https://your-domain` -> `web:80`
   - `https://your-domain/api` -> `api:8080`

## 4) Local development without Docker (optional)
### API
```bash
cd backend/src/RentFlow.Api
dotnet restore
dotnet ef database update --project ../RentFlow.Infrastructure --startup-project .
dotnet run
```

### Frontend
```bash
cd frontend
npm install
npm start
```

## Notes
- Reminder background job runs inside API service.
- Email reminders are enabled only if SMTP env vars are configured.
- Reminder architecture is extensible through `INotificationChannel` (for future WhatsApp/SMS).
