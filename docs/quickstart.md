# Quickstart

## Prerequisites
- .NET 9 SDK
- PostgreSQL
- (Optional) RabbitMQ, Docker

## Setup
1. Clone the repository
2. Configure your environment as described in the main README
3. Run migrations for AuthService and all modules:
	- `dotnet ef database update --project Fenicia.AuthService`
	- `dotnet ef database update --project Fenicia.Module.Basic` (repeat for each module)
4. Start the services (each module is a microservice):
	- `dotnet run --project Fenicia.AuthService`
	- `dotnet run --project Fenicia.Module.Basic` (repeat for each module)
5. (Optional) Start RabbitMQ and Docker Compose for full stack

## Useful Links
- [Main README](../README.md)
- [Architecture](architecture.md)
- [Modules](modules.md)
- [API Reference](api.md)
