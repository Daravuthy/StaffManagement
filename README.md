# Staff Management

Simple staff management assignment built with `ASP.NET Core Web API` for the backend and `Blazor WebAssembly` for the front end.

## Features

- Staff CRUD
- Search by staff ID and full name
- Advanced search by gender and birthday range
- Export filtered results to Excel and PDF
- Unit, integration, and end-to-end tests

## Solution Structure

- `src/StaffManagement.Api`: Web API, SQLite, Excel/PDF export
- `src/StaffManagement.Shared`: shared DTOs and request models
- `src/StaffManagement.Web`: Blazor WebAssembly UI
- `tests/StaffManagement.UnitTests`: service-level tests
- `tests/StaffManagement.IntegrationTests`: API integration tests
- `tests/StaffManagement.EndToEndTests`: browser tests with Playwright

## Run Locally

1. Restore the solution:

```powershell
dotnet restore .\StaffManagement\StaffManagement.sln
```

2. Run the API:

```powershell
dotnet run --launch-profile http --project .\StaffManagement\src\StaffManagement.Api\StaffManagement.Api.csproj
```

3. Run the Blazor app in a second terminal:

```powershell
dotnet run --launch-profile http --project .\StaffManagement\src\StaffManagement.Web\StaffManagement.Web.csproj
```

4. Open `http://localhost:5165/staffs`

The API runs on `http://localhost:5144` and the Blazor app is already configured to call that base URL.

## Test Commands

```powershell
dotnet test .\StaffManagement\tests\StaffManagement.UnitTests\StaffManagement.UnitTests.csproj
dotnet test .\StaffManagement\tests\StaffManagement.IntegrationTests\StaffManagement.IntegrationTests.csproj
dotnet test .\StaffManagement\tests\StaffManagement.EndToEndTests\StaffManagement.EndToEndTests.csproj
```

Before end-to-end tests, install the Playwright browser runtime:

```powershell
pwsh .\StaffManagement\tests\StaffManagement.EndToEndTests\bin\Debug\net9.0\playwright.ps1 install
```

## CI

The GitHub Actions workflow for this assignment currently validates `restore` and `build` on Windows only.
Tests are kept as local verification steps and are not required to run in CI for this submission.

## Technical Notes

- Persistence uses SQLite for a simple submission setup.
- The database is created automatically on startup with `EnsureCreated()`.
- Reports are generated on the API side after applying the current search filters.
- CORS is open for simplicity in local development.
