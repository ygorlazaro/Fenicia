# Fenicia Platform - Docker Setup Guide

## 📋 Overview

This Docker setup provides a complete development and production environment for the Fenicia Platform, including:

- **PostgreSQL** - Main database
- **Redis** - Caching and session storage
- **Seq** - Centralized logging
- **Fenicia.Auth** - Authentication service
- **Fenicia.Module.Basic** - Basic module (Customers, Products, etc.)
- **Fenicia.Module.Projects** - Projects module
- **Fenicia.Module.\*** - All other modules
- **fenicia-web** - React frontend

## 🚀 Quick Start

### Prerequisites

- Docker Desktop (or Docker + Docker Compose)
- .NET 10 SDK (for local development)
- Node.js 20+ (for local frontend development)
- VS Code or Rider (for debugging)

### Starting All Services

```bash
# From the project root directory
cd /home/ygor/Projects/Fenicia

# Build and start all services
docker-compose -f Docker/docker-compose.yml up -d --build

# View logs
docker-compose -f Docker/docker-compose.yml logs -f

# Stop all services
docker-compose -f Docker/docker-compose.yml down
```

### Starting Specific Services

```bash
# Start only infrastructure (PostgreSQL, Redis, Seq)
docker-compose -f Docker/docker-compose.yml up -d postgres redis seq

# Start Auth service only
docker-compose -f Docker/docker-compose.yml up -d fenicia-auth

# Start Projects module only
docker-compose -f Docker/docker-compose.yml up -d fenicia-module-projects
```

## 🔍 Debugging Support

### VS Code

1. **Attach to Running Container:**
   - Open the Debug panel (Ctrl+Shift+D)
   - Select "Attach to Fenicia.Auth (Debug)" or any other service
   - Press F5 to start debugging

2. **Launch Local Debug:**
   - Select "Launch and Debug Fenicia.Auth"
   - Press F5

3. **Debug Multiple Services:**
   - Select "Debug All Services" compound configuration

### Rider

1. **Attach to Process:**
   - Run → Attach to Process
   - Select the Docker container process
   - Set breakpoints and debug

2. **Docker Debug Configuration:**
   - Go to Run → Edit Configurations
   - Add new ".NET Core Remote" configuration
   - Set container name and process ID

### Debugging Tips

```bash
# Check if debug ports are exposed
docker ps | grep fenicia

# View container logs
docker logs fenicia-auth -f

# Enter running container
docker exec -it fenicia-auth /bin/bash

# Restart a service after code changes
docker restart fenicia-auth
```

## 📦 Service Ports

| Service | HTTP | HTTPS | Purpose |
|---------|------|-------|---------|
| fenicia-web | 3000 | - | React frontend |
| fenicia-auth | 5000 | 5001 | Authentication |
| fenicia-module-basic | 5002 | 5003 | Basic module |
| fenicia-module-projects | 5144 | 5145 | Projects module |
| fenicia-module-accounting | 5010 | 5011 | Accounting |
| fenicia-module-contracts | 5012 | 5013 | Contracts |
| fenicia-module-customersupport | 5014 | 5015 | Customer Support |
| fenicia-module-ecommerce | 5016 | 5017 | E-commerce |
| fenicia-module-hr | 5018 | 5019 | HR |
| fenicia-module-performanceevaluation | 5020 | 5021 | Performance Evaluation |
| fenicia-module-pos | 5022 | 5023 | POS |
| fenicia-module-plus | 5024 | 5025 | Plus |
| fenicia-module-socialnetwork | 5026 | 5027 | Social Network |
| postgres | 5432 | - | Database |
| redis | 6379 | - | Cache |
| seq | 5341 | 5342 | Logging |

## 🔧 Configuration

### Environment Variables

Edit `Docker/.env` to configure:

- Database connection strings
- JWT secrets
- API base URLs
- Service ports

### Connection Strings

All services use this connection string format:
```
Host=postgres;Port=5432;Database=fenicia;Username=postgres;Password=postgres
```

## 🏗️ Building

### Build All Services

```bash
docker-compose -f Docker/docker-compose.yml build
```

### Build Specific Service

```bash
docker-compose -f Docker/docker-compose.yml build fenicia-auth
docker-compose -f Docker/docker-compose.yml build fenicia-module-projects
```

### Build with No Cache

```bash
docker-compose -f Docker/docker-compose.yml build --no-cache
```

## 📊 Health Checks

Check service health:

```bash
# PostgreSQL
docker exec fenicia-postgres pg_isready

# Redis
docker exec fenicia-redis redis-cli ping

# Auth Service
curl http://localhost:5000/health

# Seq
curl http://localhost:5341/health
```

## 💾 Data Persistence

Data is persisted in Docker volumes:

- `pgdata` - PostgreSQL database
- `redis-data` - Redis data
- `seq-data` - Seq logs

To reset all data:

```bash
docker-compose -f Docker/docker-compose.yml down -v
```

⚠️ **Warning:** This will delete all data!

## 🔒 Security Notes

### Development vs Production

The current setup is optimized for **development** with:
- Debug mode enabled
- Detailed errors enabled
- HTTP and HTTPS exposed
- Source code mounted for hot reload

For **production**, you should:
- Set `BUILD_CONFIGURATION=Release`
- Remove debug ports
- Use secrets management
- Enable HTTPS only
- Add rate limiting
- Configure proper CORS

## 🐛 Troubleshooting

### Services Won't Start

```bash
# Check logs
docker-compose -f Docker/docker-compose.yml logs fenicia-auth

# Check if ports are in use
netstat -tulpn | grep 5000

# Rebuild
docker-compose -f Docker/docker-compose.yml up -d --force-recreate --build
```

### Database Connection Issues

```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# Test connection
docker exec -it fenicia-postgres psql -U postgres -d fenicia

# View PostgreSQL logs
docker logs fenicia-postgres
```

### Frontend Can't Connect to Backend

1. Check environment variables in `fenicia-web/Dockerfile`
2. Ensure backend services are running
3. Check network connectivity:
   ```bash
   docker network inspect fenicia-network
   ```

## 📝 Additional Commands

```bash
# View running containers
docker-compose -f Docker/docker-compose.yml ps

# View resource usage
docker stats

# Clean up unused resources
docker system prune -a

# Export logs
docker-compose -f Docker/docker-compose.yml logs > logs.txt
```

## 🎯 Development Workflow

1. **Start infrastructure:**
   ```bash
   docker-compose -f Docker/docker-compose.yml up -d postgres redis seq
   ```

2. **Develop locally** with hot reload:
   - Backend: Run `dotnet watch run` in service directory
   - Frontend: Run `npm start` in fenicia-web directory

3. **Debug** using VS Code or Rider

4. **Test** changes in running containers

5. **Build and deploy** when ready:
   ```bash
   docker-compose -f Docker/docker-compose.yml up -d --build
   ```

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
