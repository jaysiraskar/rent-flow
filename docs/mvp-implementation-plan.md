# RentFlow MVP Implementation Plan

## 1) Final Folder Structure (MVP)

```text
rent-flow/
├─ backend/
│  ├─ RentFlow.sln
│  ├─ src/
│  │  ├─ RentFlow.Api/
│  │  │  ├─ Controllers/
│  │  │  │  ├─ AuthController.cs
│  │  │  │  ├─ DashboardController.cs
│  │  │  │  ├─ PropertiesController.cs
│  │  │  │  ├─ TenantsController.cs
│  │  │  │  ├─ RentRecordsController.cs
│  │  │  │  └─ RemindersController.cs
│  │  │  ├─ BackgroundJobs/
│  │  │  │  └─ RentReminderHostedService.cs
│  │  │  ├─ Middlewares/
│  │  │  │  └─ GlobalExceptionMiddleware.cs
│  │  │  ├─ Extensions/
│  │  │  │  ├─ ServiceCollectionExtensions.cs
│  │  │  │  └─ ApplicationBuilderExtensions.cs
│  │  │  ├─ Program.cs
│  │  │  ├─ appsettings.json
│  │  │  ├─ appsettings.Development.json
│  │  │  └─ Dockerfile
│  │  ├─ RentFlow.Application/
│  │  │  ├─ DTOs/
│  │  │  │  ├─ Auth/
│  │  │  │  ├─ Dashboard/
│  │  │  │  ├─ Properties/
│  │  │  │  ├─ Tenants/
│  │  │  │  ├─ RentRecords/
│  │  │  │  └─ Reminders/
│  │  │  ├─ Interfaces/
│  │  │  │  ├─ IAuthService.cs
│  │  │  │  ├─ IPropertyService.cs
│  │  │  │  ├─ ITenantService.cs
│  │  │  │  ├─ IRentRecordService.cs
│  │  │  │  ├─ IDashboardService.cs
│  │  │  │  ├─ IReminderService.cs
│  │  │  │  └─ INotificationChannel.cs
│  │  │  ├─ Services/
│  │  │  ├─ Validators/
│  │  │  └─ Mappings/
│  │  ├─ RentFlow.Domain/
│  │  │  ├─ Entities/
│  │  │  │  ├─ User.cs
│  │  │  │  ├─ Property.cs
│  │  │  │  ├─ Tenant.cs
│  │  │  │  ├─ RentRecord.cs
│  │  │  │  └─ ReminderLog.cs
│  │  │  ├─ Enums/
│  │  │  │  ├─ RentPaymentStatus.cs
│  │  │  │  ├─ ReminderType.cs
│  │  │  │  └─ ReminderChannel.cs
│  │  │  └─ Common/
│  │  │     └─ BaseEntity.cs
│  │  └─ RentFlow.Infrastructure/
│  │     ├─ Data/
│  │     │  ├─ AppDbContext.cs
│  │     │  ├─ Configurations/
│  │     │  ├─ Seed/
│  │     │  └─ Migrations/
│  │     ├─ Auth/
│  │     │  ├─ JwtTokenGenerator.cs
│  │     │  └─ PasswordHasher.cs
│  │     ├─ Reminders/
│  │     │  ├─ EmailNotificationChannel.cs
│  │     │  └─ ReminderDispatcher.cs
│  │     └─ Repositories/
│  ├─ tests/
│  │  └─ RentFlow.Tests/
│  │     ├─ Unit/
│  │     │  ├─ RentStatusCalculatorTests.cs
│  │     │  └─ DashboardAggregationTests.cs
│  │     ├─ Fixtures/
│  │     └─ RentFlow.Tests.csproj
│  └─ .dockerignore
├─ frontend/
│  ├─ src/
│  │  ├─ app/
│  │  │  ├─ core/
│  │  │  │  ├─ guards/auth.guard.ts
│  │  │  │  ├─ interceptors/auth.interceptor.ts
│  │  │  │  └─ services/api-client.service.ts
│  │  │  ├─ shared/
│  │  │  │  ├─ models/
│  │  │  │  └─ components/
│  │  │  ├─ layout/
│  │  │  │  ├─ app-shell.component.ts
│  │  │  │  └─ sidebar.component.ts
│  │  │  ├─ features/
│  │  │  │  ├─ auth/
│  │  │  │  │  ├─ login.component.ts
│  │  │  │  │  ├─ register.component.ts
│  │  │  │  │  └─ auth.service.ts
│  │  │  │  ├─ dashboard/
│  │  │  │  │  ├─ dashboard.page.ts
│  │  │  │  │  └─ dashboard.service.ts
│  │  │  │  ├─ properties/
│  │  │  │  │  ├─ property-list.page.ts
│  │  │  │  │  ├─ property-detail.page.ts
│  │  │  │  │  └─ properties.service.ts
│  │  │  │  ├─ tenants/
│  │  │  │  │  ├─ tenant-list.page.ts
│  │  │  │  │  ├─ tenant-form.component.ts
│  │  │  │  │  └─ tenants.service.ts
│  │  │  │  └─ rent-records/
│  │  │  │     ├─ rent-record-list.page.ts
│  │  │  │     └─ rent-records.service.ts
│  │  │  ├─ app.routes.ts
│  │  │  └─ app.component.ts
│  │  ├─ environments/
│  │  ├─ index.html
│  │  └─ main.ts
│  ├─ Dockerfile
│  ├─ nginx.conf
│  └─ .dockerignore
├─ docker-compose.yml
├─ README.md
└─ docs/
   └─ mvp-implementation-plan.md
```

---

## 2) Database Design (SQL Server via EF Core)

### Tables and fields

1. **Users**
- `Id` (uniqueidentifier, PK)
- `FullName` (nvarchar(120), required)
- `Email` (nvarchar(160), required, unique)
- `PasswordHash` (nvarchar(max), required)
- `PhoneNumber` (nvarchar(20), optional)
- `CreatedAtUtc` (datetime2)
- `UpdatedAtUtc` (datetime2)

2. **Properties**
- `Id` (uniqueidentifier, PK)
- `LandlordId` (uniqueidentifier, FK -> Users.Id, required)
- `Name` (nvarchar(120), required)
- `AddressLine1` (nvarchar(200), required)
- `City` (nvarchar(100), required)
- `State` (nvarchar(100), required)
- `Pincode` (nvarchar(10), required)
- `CreatedAtUtc` / `UpdatedAtUtc`

3. **Tenants**
- `Id` (uniqueidentifier, PK)
- `PropertyId` (uniqueidentifier, FK -> Properties.Id, required)
- `FullName` (nvarchar(120), required)
- `Phone` (nvarchar(20), required)
- `Email` (nvarchar(160), optional)
- `RoomOrBed` (nvarchar(50), required)
- `MonthlyRent` (decimal(18,2), required)
- `RentDueDay` (tinyint, required, 1-28 validation)
- `MoveInDate` (date, required)
- `IsActive` (bit, default 1)
- `CreatedAtUtc` / `UpdatedAtUtc`

4. **RentRecords**
- `Id` (uniqueidentifier, PK)
- `TenantId` (uniqueidentifier, FK -> Tenants.Id, required)
- `PropertyId` (uniqueidentifier, FK -> Properties.Id, required, denormalized for filtering)
- `BillingYear` (smallint, required)
- `BillingMonth` (tinyint, required)
- `DueDate` (date, required)
- `ExpectedAmount` (decimal(18,2), required)
- `PaidAmount` (decimal(18,2), required default 0)
- `Status` (tinyint, required; enum: unpaid/partial/paid)
- `PaidOnUtc` (datetime2, optional)
- `Notes` (nvarchar(500), optional)
- `CreatedAtUtc` / `UpdatedAtUtc`

5. **ReminderLogs**
- `Id` (uniqueidentifier, PK)
- `RentRecordId` (uniqueidentifier, FK -> RentRecords.Id, required)
- `TenantId` (uniqueidentifier, FK -> Tenants.Id, required)
- `Channel` (tinyint, required; enum: email now, whatsapp/sms later)
- `ReminderType` (tinyint, required; upcoming/overdue)
- `Recipient` (nvarchar(160), required)
- `Message` (nvarchar(1000), required)
- `SentAtUtc` (datetime2, required)
- `Success` (bit, required)
- `FailureReason` (nvarchar(500), optional)

### Indexes
- `Users(Email)` unique
- `Properties(LandlordId)`
- `Tenants(PropertyId, IsActive)`
- `RentRecords(PropertyId, BillingYear, BillingMonth, Status)`
- `RentRecords(TenantId, BillingYear, BillingMonth)` unique (prevents duplicate month generation)
- `ReminderLogs(RentRecordId, SentAtUtc)`

### Relationship rules
- One landlord (`User`) has many properties.
- One property has many tenants.
- One tenant has many monthly rent records.
- One rent record has many reminder logs.
- Deleting a property should be restricted while active tenants exist (soft delete preferred for MVP safety).

---

## 3) API Routes (MVP)

Base: `/api/v1`

### Auth
- `POST /auth/register`
- `POST /auth/login`
- `GET /auth/me`

### Properties
- `GET /properties`
- `POST /properties`
- `GET /properties/{propertyId}`
- `PUT /properties/{propertyId}`
- `DELETE /properties/{propertyId}`

### Tenants
- `GET /properties/{propertyId}/tenants`
- `POST /properties/{propertyId}/tenants`
- `GET /tenants/{tenantId}`
- `PUT /tenants/{tenantId}`
- `DELETE /tenants/{tenantId}`

### Rent Records
- `POST /rent-records/generate-monthly?year=YYYY&month=MM&propertyId={optional}`
- `GET /rent-records?year=YYYY&month=MM&propertyId={id?}&status={unpaid|partial|paid}`
- `GET /rent-records/{rentRecordId}`
- `PUT /rent-records/{rentRecordId}/payment`
  - body: `{ paidAmount, paidOnUtc?, notes? }`
- `PUT /rent-records/{rentRecordId}/status`
  - manual override only for MVP admin correction

### Dashboard
- `GET /dashboard/summary?year=YYYY&month=MM&propertyId={optional}`
  - returns total tenants, total due, collected amount, pending amount
- `GET /dashboard/upcoming-dues?days=7`
- `GET /dashboard/overdue`

### Reminders
- `POST /reminders/run-now` (manual trigger for MVP)
- `GET /reminders/logs?year=YYYY&month=MM&propertyId={optional}`

---

## 4) Angular Page Structure (MVP)

### Public pages
- `/login`
- `/register`

### Authenticated pages
- `/dashboard`
- `/properties`
- `/properties/:id`
- `/properties/:id/tenants`
- `/tenants/:id/edit`
- `/rent-records`
- `/reminders/logs` (simple table view)

### Feature-level UI components
- Dashboard cards: Total Tenants, Total Due, Collected, Pending
- Rent records table with filters: Month, Property, Status
- Tenant form component reused for create/edit
- Property detail page with embedded tenant list + quick actions

### API integration approach
- One service per feature module (`auth.service`, `dashboard.service`, etc.)
- Shared `AuthInterceptor` to attach JWT
- `AuthGuard` for private routes
- Shared typed interfaces under `shared/models`

---

## 5) MVP Boundaries (explicitly out of scope now)
- Multi-user role model (only landlord user type in MVP)
- Online payment gateway integration
- Advanced analytics / charts beyond summary cards and tables
- WhatsApp/SMS sending implementation (design extension points only)
- Multi-language UI and tenant portal

---

## 6) Delivery Sequence After Approval
1. Implement Domain + EF Core DbContext + first migration + seed data
2. Implement JWT auth + password hashing + auth endpoints
3. Implement properties and tenants CRUD + validation
4. Implement monthly rent record generation + payment status logic
5. Implement dashboard aggregation and rent-record filters
6. Implement reminder hosted service (email channel + logs)
7. Wire Angular pages and API services
8. Add unit tests for rent status and dashboard calculations
9. Finalize Docker, compose, and README runbook
