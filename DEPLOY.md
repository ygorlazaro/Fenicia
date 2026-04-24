# Azure App Services Deployment Guide

This guide covers deploying the Fenicia Platform to **Azure App Services** using **GitHub Actions CI/CD** with **Azure Container Registry (ACR)**.

> **Scope:** This initial deployment covers the main backend services (`Fenicia.Auth`, `Fenicia.Module.Basic`) and the React frontend. Additional backend modules (Projects, HR, Accounting, etc.) can be deployed later using the same pattern.

---

## Architecture Overview

```
┌──────────────────────────────────────────────────────────────────┐
│                         Azure                                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │  Azure Static   │  │  Azure App      │  │  Azure App      │  │
│  │  Web Apps       │  │  Service        │  │  Service        │  │
│  │  (React SPA)    │  │  fenicia-auth   │  │  fenicia-basic  │  │
│  │                 │  │                 │  │                 │  │
│  │  fenicia-web    │  │  Docker         │  │  Docker         │  │
│  │  .azurestatic.  │  │  Container      │  │  Container      │  │
│  │     apps.net    │  │                 │  │                 │  │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘  │
│           │                    │                    │           │
│           │                    │     HTTPS          │           │
│   ┌───────▼────────────────────▼────────────────────▼───────┐   │
│   │              Azure Container Registry (ACR)             │   │
│   │          feniciacr.azurecr.io                            │   │
│   │  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐    │   │
│   │  │fenicia-auth  │  │fenicia-basic │  │ (future)    │    │   │
│   │  │:latest       │  │:latest       │  │  modules    │    │   │
│   │  └──────────────┘  └──────────────┘  └─────────────┘    │   │
│   └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│   ┌─────────────────────────────────────────────────────────┐   │
│   │                Azure Database for PostgreSQL             │   │
│   │              Flexible Server                             │   │
│   └─────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

---

## Prerequisites

### 1. Azure Resources to Create

| Resource | Purpose | Recommended Tier |
|----------|---------|-----------------|
| **Resource Group** | Logical grouping of all resources | - |
| **Azure Container Registry (ACR)** | Store Docker images | Basic or Standard |
| **Azure App Service Plan (Linux)** | Host .NET containers | B1 or B2 (Standard) |
| **Azure App Service (2x)** | `fenicia-auth-api`, `fenicia-basic-api` | Linux Container |
| **Azure Static Web Apps** OR **Azure App Service** | React frontend | Free tier (SWA) or B1 |
| **Azure Database for PostgreSQL** | Shared database | Flexible Server, Burstable B1ms |

### 2. Required Tools

- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli)
- [Docker](https://docs.docker.com/get-docker/)
- GitHub repository access

### 3. GitHub Secrets to Configure

Navigate to **Settings > Secrets and variables > Actions** in your GitHub repository.

| Secret Name | Description | How to Get |
|-------------|-------------|-----------|
| `AZURE_CREDENTIALS` | Service Principal JSON | `az ad sp create-for-rbac` |
| `AZURE_REGISTRY_USERNAME` | ACR username | ACR > Access Keys |
| `AZURE_REGISTRY_PASSWORD` | ACR password | ACR > Access Keys |
| `AZURE_RESOURCE_GROUP` | Resource group name | e.g., `rg-fenicia-prod` |
| `AZURE_POSTGRES_CONNECTION_STRING` | PostgreSQL connection string | Azure Portal |
| `AZURE_AUTH_API_URL` | Auth API URL | e.g., `https://fenicia-auth-api.azurewebsites.net` |
| `AZURE_BASIC_API_URL` | Basic API URL | e.g., `https://fenicia-basic-api.azurewebsites.net` |
| `AZURE_PROJECTS_API_URL` | Projects API URL | e.g., `https://fenicia-projects-api.azurewebsites.net` |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | SWA deployment token | Azure Portal > SWA > Manage deployment token |

---

## Quick Start: Automated Deployment

### Step 1: Create Azure Service Principal

```bash
# Login to Azure
az login

# Set your subscription
az account set --subscription "Your Subscription Name"

# Create Service Principal (save the output JSON!)
az ad sp create-for-rbac \
  --name "fenicia-github-actions" \
  --role contributor \
  --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/rg-fenicia-prod \
  --sdk-auth
```

Copy the JSON output and save it as `AZURE_CREDENTIALS` in GitHub secrets.

### Step 2: Run Infrastructure Deployment Script

```bash
cd /path/to/Fenicia
chmod +x infra/deploy-azure.sh

# Create resources (first time only)
./infra/deploy-azure.sh create \
  --resource-group rg-fenicia-prod \
  --location eastus \
  --postgres-password "YourStrongPassword123!"

# Deploy application (after infra is created)
./infra/deploy-azure.sh deploy \
  --resource-group rg-fenicia-prod \
  --auth-name fenicia-auth-api \
  --basic-name fenicia-basic-api
```

### Step 3: Configure GitHub Secrets

Add all secrets listed in the Prerequisites table above. Then push to `develop` or `main` branch — deployments will trigger automatically.

---

## Manual Deployment (Step-by-Step)

### 1. Create Azure Container Registry

```bash
az acr create \
  --resource-group rg-fenicia-prod \
  --name feniciacr \
  --sku Basic \
  --admin-enabled true
```

### 2. Create PostgreSQL Database

```bash
# Create server
az postgres flexible-server create \
  --resource-group rg-fenicia-prod \
  --name fenicia-postgres \
  --location eastus \
  --admin-user feniciaadmin \
  --admin-password "YourStrongPassword123!" \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --storage-size 32 \
  --version 15

# Allow Azure services access
az postgres flexible-server firewall-rule create \
  --resource-group rg-fenicia-prod \
  --name fenicia-postgres \
  --rule-name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### 3. Create App Service Plan

```bash
az appservice plan create \
  --resource-group rg-fenicia-prod \
  --name asp-fenicia-prod \
  --location eastus \
  --sku B1 \
  --is-linux
```

### 4. Create Web Apps for Backends

```bash
# Auth Web App
az webapp create \
  --resource-group rg-fenicia-prod \
  --plan asp-fenicia-prod \
  --name fenicia-auth-api \
  --deployment-container-image-name feniciacr.azurecr.io/fenicia-auth:latest

# Basic Web App
az webapp create \
  --resource-group rg-fenicia-prod \
  --plan asp-fenicia-prod \
  --name fenicia-basic-api \
  --deployment-container-image-name feniciacr.azurecr.io/fenicia-module-basic:latest
```

### 5. Configure Web App Settings

```bash
# Connection string (same for both services)
az webapp config appsettings set \
  --resource-group rg-fenicia-prod \
  --name fenicia-auth-api \
  --settings \
    "ConnectionStrings__Auth=Host=fenicia-postgres.postgres.database.azure.com;Port=5432;Database=fenicia;Username=feniciaadmin;Password=YourStrongPassword123!;SslMode=Require" \
    "DOTNET_ENVIRONMENT=Production" \
    "ASPNETCORE_URLS=http://+:8080"

az webapp config appsettings set \
  --resource-group rg-fenicia-prod \
  --name fenicia-basic-api \
  --settings \
    "ConnectionStrings__Auth=Host=fenicia-postgres.postgres.database.azure.com;Port=5432;Database=fenicia;Username=feniciaadmin;Password=YourStrongPassword123!;SslMode=Require" \
    "DOTNET_ENVIRONMENT=Production" \
    "ASPNETCORE_URLS=http://+:8080"
```

### 6. Create Static Web App for Frontend

```bash
az staticwebapp create \
  --name fenicia-web \
  --resource-group rg-fenicia-prod \
  --location eastus \
  --source https://github.com/YOUR_ORG/YOUR_REPO \
  --branch main \
  --app-location "fenicia-web" \
  --output-location "build"
```

Or deploy manually via GitHub Actions (recommended for more control).

---

## CI/CD Pipeline Details

### Frontend Deployment (`deploy-frontend.yml`)

| Trigger | Paths |
|---------|-------|
| Push to `main` or `develop` | `fenicia-web/**`, workflow file |
| Manual | Via GitHub Actions UI |

**Steps:**
1. Checkout code
2. Setup Node.js 20 with npm caching
3. Install dependencies (`npm ci`)
4. Build production bundle with environment variables
5. Deploy to Azure Static Web Apps

### Auth Service Deployment (`deploy-auth.yml`)

**Steps:**
1. Build Docker image from `Fenicia.Auth/Dockerfile`
2. Push to ACR with Git SHA and `latest` tags
3. Deploy image to Azure Web App (container)
4. Update application settings

### Basic Module Deployment (`deploy-basic.yml`)

Same pattern as Auth, but for `Fenicia.Module.Basic`.

---

## Environment Configuration

### Frontend Environment Variables

| Variable | Development | Production |
|----------|------------|------------|
| `VITE_AUTH_API_BASE_URL` | `http://localhost:5144` | `https://fenicia-auth-api.azurewebsites.net` |
| `VITE_BASIC_API_BASE_URL` | `http://localhost:5000` | `https://fenicia-basic-api.azurewebsites.net` |

> These are set in GitHub secrets and injected at build time.

### Backend Environment Variables

| Variable | Description |
|----------|-------------|
| `DOTNET_ENVIRONMENT` | `Production` |
| `ConnectionStrings__Auth` | PostgreSQL connection string |
| `ASPNETCORE_URLS` | `http://+:8080` (containers listen on 8080) |

---

## CORS Configuration

Update `Program.cs` in `Fenicia.Auth` and `Fenicia.Module.Basic` to allow the frontend origin:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins(
                "https://fenicia-web.azurestaticapps.net",
                "https://www.yourcustomdomain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

---

## Adding More Backend Modules Later

To deploy additional modules (e.g., `Fenicia.Module.Projects`):

1. **Create new GitHub workflow** (copy from `deploy-basic.yml`)
2. **Update docker-compose.prod.yml** to include the new service
3. **Add GitHub secrets** for the new API URL
4. **Update frontend `.env.production`** with the new API URL
5. **Add Bicep module** to `infra/main.bicep`

---

## Monitoring & Logging

### Azure Monitor

```bash
# Enable application insights (create once)
az resource create \
  --resource-group rg-fenicia-prod \
  --name fenicia-appinsights \
  --resource-type "Microsoft.Insights/components" \
  --properties '{"Application_Type":"web","Location":"eastus","WorkspaceResourceId":""}'
```

### Application Insights

Add the Application Insights SDK to `Program.cs`:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### View Logs

```bash
# Stream logs from a web app
az webapp log tail --name fenicia-auth-api --resource-group rg-fenicia-prod

# View recent logs
az webapp log download --name fenicia-auth-api --resource-group rg-fenicia-prod
```

---

## Cost Estimation (Monthly, USD)

| Resource | Tier | Est. Monthly Cost |
|----------|------|------------------|
| App Service Plan | B1 (Linux) | ~$13 |
| PostgreSQL Flexible | B1ms Burstable | ~$15 |
| Container Registry | Basic | ~$5 |
| Static Web Apps | Free | $0 |
| Application Insights | Basic | ~$2 |
| **Total** | | **~$35/month** |

For production workloads, consider upgrading to:
- **App Service Plan**: P1v2 (~$73/month) for better performance
- **PostgreSQL**: GP_Standard_D2s_v3 (~$65/month) for higher availability

---

## Troubleshooting

### Container fails to start

```bash
# Check container logs
az webapp log tail --name fenicia-auth-api --resource-group rg-fenicia-prod

# Check deployment logs
az webapp deployment container show-cd-url --name fenicia-auth-api --resource-group rg-fenicia-prod
```

### Database connection issues

1. Verify firewall rules allow Azure services
2. Check connection string format (must include `SslMode=Require`)
3. Ensure database exists: connect via pgAdmin/Azure Data Studio

### CORS errors in browser

1. Verify `ProductionCors` policy includes frontend URL
2. Check if `AllowCredentials()` is needed for auth cookies

---

## Security Best Practices

1. **Use Managed Identity** where possible instead of connection strings
2. **Enable HTTPS Only** on all App Services
3. **Use Key Vault** for secrets (advanced)
4. **Enable Azure Defender** for containers and databases
5. **Regularly rotate** ACR credentials and database passwords
6. **Use private endpoints** for database access (production)

---

## Useful Commands

```bash
# List all resources
az resource list --resource-group rg-fenicia-prod --output table

# Restart a web app
az webapp restart --name fenicia-auth-api --resource-group rg-fenicia-prod

# Scale up App Service Plan
az appservice plan update --name asp-fenicia-prod --resource-group rg-fenicia-prod --sku S1

# Delete all resources
az group delete --name rg-fenicia-prod --yes
```
