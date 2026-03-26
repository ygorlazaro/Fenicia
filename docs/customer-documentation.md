# Fenicia - Business Management Platform

Fenicia is a multi-tenant SaaS platform that provides comprehensive business management tools for companies of all sizes. With modular architecture, you only pay for the features your business needs.

---

## What is Fenicia?

Fenicia is a cloud-based business management platform designed to streamline your company's daily operations. Whether you need to manage customers, products, inventory, or orders, Fenicia provides an all-in-one solution with powerful analytics and reporting capabilities.

### Key Benefits

- **Modular Design**: Subscribe only to the modules you need
- **Multi-tenant Security**: Your data is isolated and protected
- **Real-time Analytics**: Make data-driven decisions with built-in insights
- **Scalable Architecture**: Grows with your business

---

## Platform Modules

### Basic Module (Core Business Operations)

The Basic module provides essential tools for managing your business operations:

| Feature | Description |
|---------|-------------|
| **Customers** | Manage customer profiles, contact information, and track purchase history |
| **Products** | Product catalog with pricing, categories, and supplier management |
| **Orders** | Complete order management from creation to fulfillment |
| **Inventory** | Real-time stock tracking, dashboards, and health analysis |
| **Employees** | Employee records, positions, and performance tracking |
| **Suppliers** | Supplier management and relationship tracking |

---

## Feature Highlights

### Customer Management
- Complete customer profiles with contact details
- Purchase history and order tracking
- Customer insights and analytics
- Risk alerts for customer churn prevention
- Customer lifetime value analysis

### Product Management
- Product catalog with categories and suppliers
- Pricing management (cost and sales price)
- Performance analytics (best/worst sellers)
- Profit margin analysis
- Never-sold product alerts

### Order Management
- Create and manage product orders
- Track order status (Approved, Pending, Cancelled)
- Order analytics and trends
- Top customer identification
- Sales performance metrics

### Inventory Control
- Real-time stock levels
- Inventory dashboards with key metrics
- Low stock alerts
- Overstock detection
- Zero-movement product identification
- Category and supplier breakdowns
- Profit potential calculations

### Employee Management
- Employee profiles with positions
- Performance tracking and analytics
- Sales by employee reporting
- Top performer identification

---

## How It Works

### 1. Company Setup
Register your company and configure your organization in the platform.

### 2. Choose Modules
Select the modules that match your business needs. You can add more modules anytime.

### 3. Invite Users
Add your team members and assign appropriate roles.

### 4. Start Managing
Begin using the platform to manage your business operations.

---

## Subscription Model

Fenicia operates on a subscription-based model:

1. **Company Registration** - Create your company account
2. **Module Selection** - Choose which modules you need
3. **Subscription** - Activate your subscription to access modules
4. **Add Users** - Invite team members to join your company

Your subscription determines which features are available to your team.

---

## Security & Data Protection

### Authentication
- Secure JWT-based authentication
- Session management with automatic expiration
- Brute-force protection with login attempt tracking

### Authorization
- Role-based access control (RBAC)
- Company-level permission isolation
- Module-level access control

### Data Isolation
- Each company can only access its own data
- All queries automatically filter by your company
- Subscription validation for module access

---

## API Overview

The platform provides RESTful APIs for integration:

### Customer Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/customer` | List customers (paginated) |
| GET | `/customer/{id}` | Get customer details |
| POST | `/customer` | Create customer |
| PATCH | `/customer/{id}` | Update customer |
| DELETE | `/customer/{id}` | Delete customer |
| GET | `/customer/insights` | Customer analytics |

### Product Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/product` | List products (paginated) |
| GET | `/product/{id}` | Get product details |
| POST | `/product` | Create product |
| PATCH | `/product/{id}` | Update product |
| DELETE | `/product/{id}` | Delete product |
| GET | `/product/performance` | Product analytics |

### Order Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/order` | List orders (paginated) |
| GET | `/order/{id}` | Get order details |
| POST | `/order` | Create order |
| DELETE | `/order/{id}` | Delete order |
| GET | `/order/analytics` | Order analytics |

### Inventory Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/inventory` | List inventory |
| GET | `/inventory/dashboard` | Inventory dashboard |
| GET | `/inventory/health` | Inventory health analysis |

### Employee Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/employee` | List employees |
| GET | `/employee/{id}` | Get employee details |
| POST | `/employee` | Create employee |
| PATCH | `/employee/{id}` | Update employee |
| DELETE | `/employee/{id}` | Delete employee |
| GET | `/employee/performance` | Performance analytics |

---

## Getting Started

### For Administrators

1. **Configure Company Settings**
   - Set up company information
   - Manage user accounts
   - Assign roles to team members

2. **Manage Subscriptions**
   - View available modules
   - Purchase or upgrade subscriptions
   - Monitor subscription status

3. **Configure Data**
   - Set up product categories
   - Define employee positions
   - Add suppliers

### For Team Members

1. **Log In** - Access the platform with your credentials
2. **Navigate** - Use the dashboard to access your assigned modules
3. **Work** - Perform your daily tasks within your role permissions

---

## Need Help?

For technical support or questions about the platform, contact your system administrator or reach out to our support team.

---

*Last updated: March 2026*
