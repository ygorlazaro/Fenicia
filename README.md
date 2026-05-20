

# Fenicia SaaS Platform

<a href="https://discord.gg/RNuSz2t4wm" target="_blank"><img src="https://img.shields.io/discord/1245739632657489950?label=Join%20our%20Discord&logo=discord&style=for-the-badge" alt="Discord"></a>

![Build Status](https://img.shields.io/github/actions/workflow/status/ygorlazaro/Fenicia/ci.yml?branch=develop&style=for-the-badge)
![Tests](https://img.shields.io/badge/tests-passing-brightgreen?style=for-the-badge)
![Coverage](https://img.shields.io/badge/coverage-unknown-lightgrey?style=for-the-badge)

---

**[CONTRIBUTING.md](CONTRIBUTING.md) | [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) | [LICENSE](LICENSE) | [SECURITY.md](SECURITY.md) | [LINKEDIN.MD](LINKEDIN.MD) | [LINKEDIN_EN.MD](LINKEDIN_EN.MD) | [Docs](docs/README.md)**

---

## �️ Macro System Overview

Fenicia is a modular, multi-tenant SaaS platform for administrative ERP, client management, subscriptions, payments, billing, and permissions.

- **ERP Admin Portal**: Blazor WASM/MVC for SaaS management
- **Auth Service**: Central login, JWT, permissions, tenantId, active modules
- **Independent Modules**: Each is a REST microservice with its own database per tenant (`tenant_{id}_{module}`)
- **Communication**: RabbitMQ for events, JWT for authorization

![Macro Architecture](docs/architecture-diagram.svg)

---

## 🗂️ Project Organization

```
/src
  Fenicia.AdminPortal (Blazor WASM / MVC - SaaS Management)
  Fenicia.AuthService (API)
  Fenicia.Module.Basic (API)
  Fenicia.Module.Social (API)
  Fenicia.Module.Projects (API)
  Fenicia.Module.Performance (API)
  Fenicia.Module.Accounts (API)
  Fenicia.Module.HR (API)
  Fenicia.Module.PDV (API)
  Fenicia.Module.Contracts (API)
  Fenicia.Module.Ecommerce (API)
  Fenicia.Module.Support (API)
  Fenicia.Module.Plus (API)
  Fenicia.Common (DTOs, Events, Utils)
  Fenicia.BuildingBlocks (Middlewares, Bus, Multi-tenancy utilities)
  Fenicia.IntegrationTests
```

---

## 🛠️ Technical Stack

- **Backend**: .NET 10 API
- **Database**: PostgreSQL
- **Messaging**: RabbitMQ
- **ORM**: EF Core with Multi-tenancy
- **Authentication**: Centralized JWT
- **Frontend**: Blazor/MVC (AdminPortal), other modules may have dashboards
- **Containerization**: Docker + Docker Compose
- **Observability**: Serilog, HealthChecks, Grafana, Prometheus
- **CI/CD**: GitHub Actions, Azure Pipelines

---

## 📦 Modules

- **Basic**: Clients, suppliers, employees, roles, products, categories, stock, purchases, sales, inventory
- **Social**: Followers, feed, uploads, reports, events
- **Projects**: Projects, tasks, subtasks, sprints
- **Performance**: Employee evaluations, analytics
- **Accounts**: Payables/receivables, cash, transfers, recurring payments
- **HR**: Candidates, recruitment, tests, notifications
- **POS**: Point of sale, transactions, receipts
- **Contracts**: Contracts, clauses, versions, signatures
- **Ecommerce**: Online sales, tracking, notifications
- **Support**: Tickets, FAQ
- **Plus**: Services, documents, CMS, landing pages

---

## 🧩 Multi-Tenancy & Security

- ERP manages tenants and active modules per client
- Each service receives `tenantId` via JWT and injects the connection string dynamically
- Automatic migrations for new tenants
- Billing managed by ERP (plan defines enabled modules)
- Authorization middleware checks if requested module is enabled in JWT
- JWT with `sub`, `tenantId`, `companyId`, `modules`
- Short expiration with refresh
- Rate limiting middleware
- Module/tenant permissions

---

## 🧬 Communication Between Modules

- RabbitMQ for events (e.g., client creation, sales, employee onboarding)
- Event bus standardized with contracts in Fenicia.Common

---

## 📈 Scalability

- Each module can be scaled horizontally and independently
- Critical modules (Auth, Basic, POS) can have more replicas
- Separate DB per module/tenant avoids lock/contention

---

## 🌱 Next Steps

- Automatic seeds for new tenants
- PWA for AdminPortal
- Integrated payment gateways (Stripe, Mercado Pago)
- White-label and custom domains
- Monetization via ERP billing

---

## � Conclusion

This setup ensures:
- Isolation and security for clients
- Modular, scalable growth
- Increased LTV by selling extra modules
- A robust, revenue-generating SaaS

---

## 📄 License

MIT License. See [LICENSE](LICENSE).

---

## 📚 Documentation

See [docs/README.md](docs/README.md) for detailed guides, architecture, and API reference.
