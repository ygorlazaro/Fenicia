# Fenicia.Web.E2ETests

E2E test project for Fenicia.Web using Playwright and xUnit.

## Prerequisites

- .NET 10 SDK
- Playwright browsers installed

## Setup

1. Restore and build:
   ```bash
   dotnet build Fenicia.Web.E2ETests/Fenicia.Web.E2ETests.csproj
   ```

2. Install Playwright browsers (if not already installed):
   ```bash
   cd Fenicia.Web.E2ETests
   playwright install chromium
   ```

## Running Tests

### Option 1: Standalone Fenicia.Web

1. Start Fenicia.Web:
   ```bash
   dotnet run --project Fenicia.Web/Fenicia.Web.csproj
   ```

2. Run the E2E tests:
   ```bash
   dotnet test Fenicia.Web.E2ETests/Fenicia.Web.E2ETests.csproj
   ```

### Option 2: Via Aspire AppHost

1. Start the Aspire AppHost:
   ```bash
   dotnet run --project Fenicia.Aspire/Fenicia.Aspire.AppHost/Fenicia.Aspire.AppHost.csproj
   ```

2. Note the URL assigned to `fenicia-web` in the Aspire dashboard.

3. Run the tests with the correct URL:
   ```bash
   E2E_BASE_URL=http://localhost:5xxx dotnet test Fenicia.Web.E2ETests/Fenicia.Web.E2ETests.csproj
   ```

## Configuration

| Environment Variable | Description | Default |
|----------------------|-------------|---------|
| `E2E_BASE_URL` | Base URL of the running Fenicia.Web instance | `http://localhost:5104` |

## Future: Test Database Strategy

For E2E tests that require database interaction without affecting the current database:

### Recommended Approach

1. **Separate Test Environment**: Run backend APIs against a dedicated test database.
2. **Connection String Override**: Backend APIs should support overriding the `ConnectionStrings:Auth` setting via environment variable.
3. **Database per Test Run**: Use a unique database name per test run (e.g., `fenicia_test_<guid>`) and drop it after tests complete.

### Implementation Options

- **Aspire Test AppHost**: Create a separate `Fenicia.Aspire.E2E` AppHost that provisions a fresh PostgreSQL container per test run.
- **Environment Variables**: Configure backend APIs with `ConnectionStrings__Auth` pointing to the test database.
- **Testcontainers**: Use `Testcontainers.PostgreSql` in a test fixture to spin up an ephemeral database for the test duration.

### Example Backend Override

When starting a backend API for E2E tests:
```bash
ConnectionStrings__Auth="Host=localhost;Database=fenicia_e2e_test;Username=postgres;Password=postgres" \
  dotnet run --project Fenicia.Module.Basic/Fenicia.Module.Basic.csproj
```
