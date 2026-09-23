# Pharmacy Management System

This branch transforms the original EF Core demo into a pharmacy-management system with an ASP.NET Core backend and responsive Flutter client.

## Technology
- .NET 10 ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- JWT authentication with Admin, Pharmacist, Cashier and Doctor roles
- Flutter Material 3
- xUnit backend tests and Flutter API/widget tests
- GitHub Actions CI

## Implemented workflows
- Medicine catalog with brand and generic names, strength, dosage form, category, unit of measure, manufacturer, barcode, prices, reorder level, prescription requirement, controlled flag, storage instructions and active state.
- Suppliers, patients and doctors in the domain model.
- Purchase receiving and batch creation/replenishment.
- Batch number, manufacture date, expiry date, pricing, received quantity, available quantity and quarantine state.
- FEFO dispensing at batch level. Expired and quarantined batches are excluded.
- Prescription items with dosage, frequency, duration, quantity prescribed and quantity dispensed.
- Sales, sale items, payments and batch allocations.
- Stock movements for purchases, sales, adjustments, returns, expired write-offs and damaged write-offs.
- Expiry, near-expiry and low-stock dashboard information.

## Backend
Configure SQL Server in EFCoreDemo/appsettings.json and run:
dotnet run --project EFCoreDemo

The current branch uses EnsureCreated for a clean development database. For a long-lived production deployment, create and review EF Core migrations before release.

## Flutter
From flutter_app:
flutter pub get
flutter run --dart-define=API_BASE_URL=http://localhost:5000/api

## Verification
Backend:
dotnet test tests/EFCoreDemo.Tests/EFCoreDemo.Tests.csproj

Flutter:
flutter analyze
flutter test

GitHub Actions executes both suites on every push to feature/pharmacy-management-system.

## Security
Do not use the development JWT secret in production. Supply secrets through deployment configuration, restrict CORS, require HTTPS, and bootstrap the first Admin account through a trusted deployment process rather than committing a default password.
