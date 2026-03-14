# Business Solution Documentation

## Overview

This documentation describes the multi-tenant SaaS business solution architecture. The platform consists of two main components:

1. **Auth Module** (`Fenicia.Auth`) - Core platform for authentication, authorization, and tenant management
2. **Modules** (`Fenicia.Module.*`) - Optional modules that tenants can subscribe to

---

## Architecture

### Core Platform (Auth Module)

The Auth module is the main project that handles:

- **User Authentication**: Login, logout, session management
- **Multi-tenancy**: Company/tenant management
- **Authorization**: Role-based access control
- **Module Subscriptions**: Managing which modules each company can access
- **Global Configuration**: System-wide settings

Related documentation:
- [CompanyDomain](./auth/CompanyDomain.md) - Company/tenant management
- [ModuleDomain](./auth/ModuleDomain.md) - Available modules
- [OrderDomain](./auth/OrderDomain.MD) - Module subscription orders
- [LoginAttemptDomain](./auth/LoginAttemptDomain.MD) - Brute-force protection
- [ForgotPasswordDomain](./auth/ForgotPasswordDomain.MD) - Password recovery
- [ConfigurationDomain](./auth/ConfigurationDomain.md) - Global configuration

### Extension Modules

Companies can subscribe to additional modules to extend functionality:

| Module | Description |
|--------|-------------|
| Basic | Core business operations (customers, products, orders, inventory) |

Each module operates independently and is only accessible to companies with an active subscription.

Related documentation:
- [OrderDomain](./basic/OrderDomain.MD) - Product orders management
- [CustomerDomain](./basic/CustomerDomain.MD) - Customer management
- [EmployeeDomain](./basic/EmployeeDomain.MD) - Employee management
- [ProductDomain](./basic/ProductDomain.MD) - Product catalog
- [StockDomain](./basic/StockDomain.MD) - Inventory management

---

## Subscription Flow

1. **Company Registration**: A new company is created in the system
2. **Module Selection**: The company selects desired modules
3. **Order Creation**: An order is created for the selected modules
4. **Payment Processing**: (Future) Payment integration
5. **Subscription Activation**: Subscription and credits are created
6. **Access Grant**: Users in the company can access the modules

See [OrderDomain](./auth/OrderDomain.MD) for detailed subscription order flow.

---

## Multi-Tenancy Model

### Company (Tenant)

Each company is a separate tenant with:
- Unique identifier
- CNPJ/business registration
- Active/inactive status
- Multiple users with roles

### User Roles

Users belong to companies with specific roles:
- **Admin**: Full access to company settings
- **Standard**: Regular user access
- Custom roles based on company needs

### Module Subscriptions

Companies subscribe to modules through:
- Orders created in the platform
- Active subscriptions with validity dates
- Subscription credits that grant access to specific modules

---

## Data Models

### Core Entities

```
Company (Tenant)
├── Users
│   └── UserRoles (Company-specific roles)
├── Subscriptions
│   └── SubscriptionCredits (Module access)
└── Orders
    └── OrderDetails (Ordered modules)
```

### Module Entities (Example: Basic)

```
Company
├── Customers
├── Employees
├── Products
├── Orders
│   └── OrderDetails
└── StockMovements
```

---

## API Structure

### Auth Module APIs

| Domain | Endpoint | Description |
|--------|----------|-------------|
| Company | `GET /company` | List user's companies |
| Company | `PATCH /company/{id}` | Update company (Admin) |
| Module | `GET /module` | List available modules |
| Order | `POST /order` | Create module subscription |
| Login | `POST /login` | User authentication |
| ForgotPassword | `POST /forgot-password` | Password recovery |

### Module APIs

Each module exposes its own APIs, for example Basic module:

| Domain | Endpoint | Description |
|--------|----------|-------------|
| Order | `GET/POST /order` | Manage orders |
| Customer | `GET/POST /customer` | Manage customers |
| Product | `GET/POST /product` | Manage products |
| Stock | `GET /stock` | View inventory |

---

## Security

### Authentication

- JWT-based authentication
- Session management with token expiration
- Login attempt tracking for brute-force protection

### Authorization

- Role-based access control (RBAC)
- Company-level permission isolation
- Module-level access control via subscriptions

### Data Isolation

- Each company can only access its own data
- Queries automatically filter by user's company
- Subscription validation for module access

---

## Getting Started

### For Platform Administrators

1. Configure available modules in the system
2. Set global configuration settings
3. Manage company registrations

### For Company Administrators

1. Invite users to the company
2. Assign roles to users
3. Purchase module subscriptions
4. Configure company-specific settings

### For Company Users

1. Log in with company credentials
2. Access modules based on company subscription
3. Perform business operations within assigned roles
