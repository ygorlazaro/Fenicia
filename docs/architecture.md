
# Architecture

![Macro Architecture](architecture-diagram.svg)

## Technical Stack

- **Backend**: .NET 10 API
- **Database**: PostgreSQL
- **ORM**: EF Core with Multi-tenancy
- **Authentication**: Centralized JWT
- **Frontend**: Vue.js + TypeScript
- **Containerization**: Docker + Docker Compose
- **Observability**: Serilog, HealthChecks, Seq
- **CI/CD**: GitHub Actions

## Multi-Tenancy

- Multi-tenancy is implemented via schema-level isolation in a single shared PostgreSQL database.
- Tenant isolation is enforced automatically through EF Core global query filters on `BaseCompanyModel` entities.
- JWT tokens include a `company_id` claim. The `CompanyId` can also be provided via the `Company-Id` HTTP header as a fallback for legacy endpoints; when both are present, they must match.
- Authorization middleware validates whether the requested module is enabled for the user's company.

## Scalability

- Each module can be scaled horizontally and independently
- Critical modules (Auth, Basic, POS) can have more replicas
- Single shared database per environment with row-level tenant isolation avoids lock/contention

## Security

- JWT with `sub`, `companyId`, `modules`
- Short expiration with refresh
- Rate limiting middleware
- Module/tenant permissions

---

> See the main README and [docs/README.md](README.md) for a full macro overview.
