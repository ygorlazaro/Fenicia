
# Fenicia Documentation

Welcome to the Fenicia documentation! Here you'll find guides, architecture diagrams, and usage examples.

## Table of Contents
- [Quickstart](quickstart.md)
- [Architecture](architecture.md)
- [Modules](modules.md)
- [API Reference](api.md)

---

## 1️⃣ Macro System Overview

Fenicia is a modular, multi-tenant SaaS platform designed for administrative ERP, client management, subscriptions, payments, billing, and permissions. It features:

- **Central Auth Service**: Manages login, JWT, permissions, `companyId`, and active modules.
- **Independent Modules**: Each module is a REST microservice sharing a single PostgreSQL database per environment, with row-level tenant isolation via `CompanyId` and authorized by JWT.
- **Frontend**: Vue.js + TypeScript

![Macro Architecture](architecture-diagram.svg)

---

## Project Overview
Fenicia is a modular, multi-tenant SaaS platform built with .NET and Next.js. It is designed for scalability, security, and rapid development.

---

## Contributing to Docs
Feel free to improve these docs by submitting a pull request!
