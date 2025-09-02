
# Modules

Fenicia is organized into decoupled modules (microservices), each responsible for a business domain and communicating via RabbitMQ events and JWT authentication. Each module has its own database per tenant.

## ✅ Basic Module
- CRUD for Clients, Suppliers, Employees, Roles, Products, Categories
- Stock movement, purchases, sales, inventory

## ✅ Social Network Module
- Followers, feed, uploads, reports
- Event scheduling and sharing

## ✅ Projects Module
- Projects, tasks, subtasks
- Assignees, status, sprints

## ✅ Performance Evaluation Module
- Employee evaluations
- Reports and analytics

## ✅ Accounts Module
- Payables/receivables, cash, statements
- Transfers, charges, recurring payments

## ✅ HR Module
- Candidates, recruitment processes, job openings
- Tests and notifications

## ✅ POS Module
- Point of sale, transactions, reports
- Integration with accounts, receipts/invoices

## ✅ Contracts Module
- Contracts, clauses, versions, signatures

## ✅ Ecommerce Module
- Online sales, tracking, notifications
- Online billing integrated with accounts

## ✅ Support Module
- Tickets and FAQ

## ✅ Plus Module
- Services, documents, CMS, landing pages

---

See the main README and API docs for more details.
