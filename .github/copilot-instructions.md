# Fenicia Copilot Instructions

## Big Picture Architecture
- Fenicia is a modular, multi-tenant SaaS ERP platform built with .NET (latest: .NET 9), PostgreSQL, RabbitMQ, and Docker.
- Each business domain is a microservice module (e.g., Auth, Basic, Social, Projects, HR, POS, Contracts, Ecommerce, Support, Plus).
- Every module is a REST API with its own database per tenant (`tenant_{id}_{module}`), communicating via RabbitMQ events and JWT authentication.
- Central Auth Service issues JWTs containing `tenantId`, `companyId`, enabled modules, and permissions. Modules validate JWT and check permissions via middleware.
- ERP Admin Portal (Blazor/MVC) centralizes SaaS management (clients, billing, permissions, etc.).

## Developer Workflows
- **Build/Run**: Use `dotnet run --project <ModuleName>` for each microservice. Example: `dotnet run --project Fenicia.AuthService`.
- **Migrations**: Run `dotnet ef database update --project <ModuleName>` for each module to apply DB migrations.
- **Testing**: Tests are organized per module (e.g., `Fenicia.Auth.Tests`). Use standard .NET test commands.
- **Full Stack**: Use Docker Compose (`Docker/docker-compose.yml`) to start all services and dependencies (RabbitMQ, PostgreSQL).
- **Observability**: Serilog, HealthChecks, Grafana, and Prometheus are integrated for logging and monitoring.

## Project-Specific Conventions
- **Multi-tenancy**: Connection strings are injected dynamically per request based on `tenantId` in JWT.
- **Events**: RabbitMQ is used for cross-module communication. Event contracts are standardized in `Fenicia.Common`.
- **Authorization**: Middleware checks if requested module is enabled for the tenant (from JWT claims).
- **Database**: Each module manages its own migrations and schema. No shared DB between modules.
- **Code Organization**: Each module is in its own folder with clear separation. Shared utilities and DTOs are in `Fenicia.Common`.

## Integration Points
- **RabbitMQ**: Used for events like client creation, sales, onboarding. See `Fenicia.Common` for event contracts.
- **Docker**: Use `Docker/docker-compose.yml` for local development and integration testing.
- **CI/CD**: GitHub Actions and Azure Pipelines are used for builds, tests, and deployments.

## Key Files & Directories
- `Fenicia.Auth/` – Central Auth microservice
- `Fenicia.Module.*` – Business domain modules (Basic, Social, Projects, etc.)
- `Fenicia.Common/` – Shared DTOs, events, utilities
- `Docker/docker-compose.yml` – Container orchestration
- `docs/architecture.md` – Macro architecture overview
- `docs/quickstart.md` – Setup and workflow guide
- `docs/modules.md` – Module descriptions

## Example Patterns
- **Adding a Module**: Create a new folder `Fenicia.Module.<Domain>`, implement REST API, migrations, RabbitMQ event handlers, and update Docker Compose.
- **Tenant DB Access**: Always resolve connection string from JWT `tenantId` claim.
- **Event Publishing**: Use contracts from `Fenicia.Common` and publish via RabbitMQ.

---
For more details, see `docs/README.md`, `docs/architecture.md`, and module-specific folders. Follow existing patterns for new modules and integrations.
