# Fenicia SaaS Platform

<a href="https://discord.gg/RNuSz2t4wm" target="_blank"><img src="https://img.shields.io/discord/1245739632657489950?label=Join%20our%20Discord&logo=discord&style=for-the-badge" alt="Discord"></a>

![Build Status](https://img.shields.io/github/actions/workflow/status/ygorlazaro/Fenicia/ci.yml?branch=develop&style=for-the-badge)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen?style=for-the-badge)
![Coverage](https://img.shields.io/badge/coverage-unknown-lightgrey?style=for-the-badge)

---

**[CONTRIBUTING.md](CONTRIBUTING.md) | [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) | [LICENSE](LICENSE) | [SECURITY.md](SECURITY.md) | [LINKEDIN.MD](LINKEDIN.MD) | [LINKEDIN_EN.MD](LINKEDIN_EN.MD) | [Docs](docs/README.md)**

---

## 🏢 Macro System Overview

Fenicia is a modular, multi-tenant SaaS platform for administrative ERP, client management, subscriptions, payments, billing, and permissions.

- **Auth Service**: Central login, JWT, permissions, `companyId`, and active modules
- **Independent Modules**: Each is a REST microservice (Basic, POS, HR, Accounting, Projects, etc.) sharing a single PostgreSQL database per environment, with row-level tenant isolation via `CompanyId`
- **Frontend**: React + TypeScript + Vite (`Src/Front`)

![Macro Architecture](docs/architecture-diagram.svg)

---

## 🗂️ Project Organization

```
Fenicia.sln
├── Fenicia.Auth/                    # Authentication & authorization service
├── Fenicia.Common.API/              # Shared API infrastructure (auth, rate limiting, CORS, logging)
├── Fenicia.Common.Data/             # Shared data layer (company context, base models, repositories)
├── Fenicia.Common/                  # Shared domain logic
├── Fenicia.Module.Basic/            # Core ERP module (products, orders, inventory, etc.)
├── Fenicia.Module.POS/              # Point of Sale module
├── Fenicia.Module.HR/               # Human Resources module
├── Fenicia.Module.Accounting/       # Accounting module
├── Fenicia.Module.Projects/         # Project management module
├── Fenicia.Module.Contracts/        # Contracts module
├── Fenicia.Module.Ecommerce/        # E-commerce integration module
├── Fenicia.Module.SocialNetwork/    # Social network features module
├── Fenicia.Module.CustomerSupport/  # Customer support module
├── Fenicia.Module.PerformanceEvaluation/ # Performance evaluation module
├── Fenicia.Module.Plus/             # Additional features module
├── Src/Front/                     # Frontend (React + TypeScript + Vite)
└── Docker/                          # Dockerfiles and docker-compose for all services
```

---

## 🛠️ Technical Stack

- **Backend**: .NET 10 Web API
- **Database**: PostgreSQL
- **ORM**: EF Core with global query filters for multi-tenancy
- **Authentication**: Centralized JWT with `company_id` claim
- **Frontend**: React + TypeScript + Vite
- **Containerization**: Docker + Docker Compose
- **Observability**: Serilog, Seq, HealthChecks
- **CI/CD**: GitHub Actions

---

## 📦 Modules

- **Basic**: Clients, suppliers, employees, roles, products, categories, stock, purchases, sales, inventory
- **SocialNetwork**: Followers, feed, uploads, reports, events
- **Projects**: Projects, tasks, subtasks, sprints
- **PerformanceEvaluation**: Employee evaluations, analytics
- **Accounting**: Payables/receivables, cash, transfers, recurring payments
- **HR**: Candidates, recruitment, tests, notifications
- **POS**: Point of sale, transactions, receipts
- **Contracts**: Contracts, clauses, versions, signatures
- **Ecommerce**: Online sales, tracking, notifications
- **CustomerSupport**: Tickets, FAQ
- **Plus**: Services, documents, CMS, landing pages

---

## 🧩 Multi-Tenancy & Security

- Multi-tenancy is implemented via **schema-level isolation** in a single shared PostgreSQL database (all tenants share the same database instance, separated by `schema` and filtered by `CompanyId`).
- Tenant isolation is enforced automatically through EF Core global query filters on `BaseCompanyModel` entities.
- JWT tokens include a `company_id` claim. The `CompanyId` can also be provided via the `Company-Id` HTTP header as a fallback for legacy endpoints; when both are present, they must match.
- Authorization middleware validates whether the requested module is enabled for the user's company.
- Short-lived JWT access tokens with refresh tokens stored in Redis.
- Rate limiting per client/IP.

---

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Docker & Docker Compose
- PostgreSQL 16+ (or use Docker)

### Running with Docker

From the repo root:

```bash
docker compose -f Docker/docker-compose.yml up --build
```

---

## 📈 Scalability

- Each module can be scaled horizontally and independently.
- Critical modules (Auth, Basic, POS) can have more replicas.
- Single shared database per environment with row-level tenant isolation avoids lock/contention while keeping operational overhead low.

---

## 📄 License

MIT License. See [LICENSE](LICENSE).

---

## 📚 Documentation

See [docs/README.md](docs/README.md) for detailed guides, architecture, and API reference.
