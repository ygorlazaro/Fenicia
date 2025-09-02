
# Architecture

![Macro Architecture](architecture-diagram.svg)

## Technical Stack

- **Backend**: .NET 9 API
- **Database**: PostgreSQL
- **Messaging**: RabbitMQ
- **ORM**: EF Core with Multi-tenancy
- **Authentication**: Centralized JWT
- **Frontend**: Blazor/MVC (AdminPortal), other modules may have their own dashboards
- **Containerization**: Docker + Docker Compose
- **Observability**: Serilog, HealthChecks, Grafana, Prometheus
- **CI/CD**: GitHub Actions, Azure Pipelines

## Multi-Tenancy

- ERP manages tenants and active modules per client
- Each service receives `tenantId` via JWT and injects the connection string dynamically
- Automatic migrations for new tenants
- Billing managed by ERP (plan defines enabled modules)
- Authorization middleware checks if requested module is enabled in JWT

## Scalability

- Each module can be scaled horizontally and independently
- Critical modules (Auth, Basic, POS) can have more replicas
- Separate DB per module/tenant avoids lock/contention

## Security

- JWT with `sub`, `tenantId`, `companyId`, `modules`
- Short expiration with refresh
- Rate limiting middleware
- Module/tenant permissions

---

> See the main README and [docs/README.md](README.md) for a full macro overview.
