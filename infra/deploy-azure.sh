#!/usr/bin/env bash
# =============================================================================
# Azure App Services Deployment Script for Fenicia Platform
# =============================================================================
# Usage:
#   chmod +x infra/deploy-azure.sh
#   ./infra/deploy-azure.sh
#
# Prerequisites:
#   - Azure CLI installed (https://learn.microsoft.com/cli/azure/install-azure-cli)
#   - Logged in: az login
#   - jq installed (optional, for JSON parsing)
# =============================================================================

set -euo pipefail

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------
SUBSCRIPTION_ID=""          # Optional: leave empty to use default
RESOURCE_GROUP="rg-fenicia-prod"
LOCATION="eastus"           # Change to your preferred region: brazilsouth, westeurope, etc.
ENVIRONMENT="prod"
CONTAINER_REGISTRY="feniciacr"
POSTGRES_SERVER="fenicia-postgres"
POSTGRES_ADMIN_USER="feniciaadmin"
POSTGRES_ADMIN_PASSWORD=""  # Will prompt if empty
POSTGRES_DB="fenicia"

# App Service Plan
APP_PLAN="asp-fenicia"
SKU="P1v3"                # Premium V3 for production (supports containers). For dev use "B2" Basic

# App Names (globally unique DNS names)
AUTH_APP="fenicia-auth-api"
BASIC_APP="fenicia-basic-api"
WEB_APP="fenicia-web-app" # Static Web Apps preferred; this is fallback App Service
STATIC_WEB_APP="fenicia-web-swa"

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------
info() { echo -e "\033[36m[INF]\033[0m $*"; }
ok()   { echo -e "\033[32m[OK]\033[0m  $*"; }
err()  { echo -e "\033[31m[ERR]\033[0m $*"; exit 1; }

# ---------------------------------------------------------------------------
# Validate prerequisites
# ---------------------------------------------------------------------------
info "Checking prerequisites..."
command -v az &>/dev/null || err "Azure CLI not found. Install: https://aka.ms/installazurecli"

if [[ -n "$SUBSCRIPTION_ID" ]]; then
    az account set --subscription "$SUBSCRIPTION_ID" || err "Failed to set subscription"
fi

CURRENT_SUB=$(az account show --query id -o tsv)
info "Using subscription: $CURRENT_SUB"

# ---------------------------------------------------------------------------
# Prompt for password if not set
# ---------------------------------------------------------------------------
if [[ -z "$POSTGRES_ADMIN_PASSWORD" ]]; then
    read -rsp "Enter Postgres admin password (min 8 chars): " POSTGRES_ADMIN_PASSWORD
echo
    [[ ${#POSTGRES_ADMIN_PASSWORD} -ge 8 ]] || err "Password must be at least 8 characters"
fi

# ---------------------------------------------------------------------------
# Resource Group
# ---------------------------------------------------------------------------
info "Creating resource group: $RESOURCE_GROUP"
az group create \
    --name "$RESOURCE_GROUP" \
    --location "$LOCATION" \
    --tags Environment="$ENVIRONMENT" Project=Fenicia \
    --output none
ok "Resource group created"

# ---------------------------------------------------------------------------
# Azure Container Registry
# ---------------------------------------------------------------------------
info "Creating Container Registry: $CONTAINER_REGISTRY"
if az acr show --name "$CONTAINER_REGISTRY" --resource-group "$RESOURCE_GROUP" &>/dev/null; then
    info "ACR already exists, skipping creation"
else
    az acr create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$CONTAINER_REGISTRY" \
        --sku Standard \
        --admin-enabled true \
        --location "$LOCATION" \
        --output none
    ok "Container Registry created"
fi

ACR_LOGIN_SERVER=$(az acr show --name "$CONTAINER_REGISTRY" --resource-group "$RESOURCE_GROUP" --query loginServer -o tsv)
ACR_USERNAME=$(az acr credential show --name "$CONTAINER_REGISTRY" --resource-group "$RESOURCE_GROUP" --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$CONTAINER_REGISTRY" --resource-group "$RESOURCE_GROUP" --query passwords[0].value -o tsv)
info "ACR Login Server: $ACR_LOGIN_SERVER"

# ---------------------------------------------------------------------------
# Azure Database for PostgreSQL - Flexible Server
# ---------------------------------------------------------------------------
info "Creating PostgreSQL Flexible Server: $POSTGRES_SERVER"
if az postgres flexible-server show --name "$POSTGRES_SERVER" --resource-group "$RESOURCE_GROUP" &>/dev/null; then
    info "PostgreSQL server already exists, skipping creation"
else
    az postgres flexible-server create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$POSTGRES_SERVER" \
        --location "$LOCATION" \
        --admin-user "$POSTGRES_ADMIN_USER" \
        --admin-password "$POSTGRES_ADMIN_PASSWORD" \
        --sku-name Standard_B1ms \
        --tier Burstable \
        --storage-size 32 \
        --version 16 \
        --database-name "$POSTGRES_DB" \
        --public-access Enabled \
        --output none
    ok "PostgreSQL server created"
fi

POSTGRES_FQDN=$(az postgres flexible-server show --name "$POSTGRES_SERVER" --resource-group "$RESOURCE_GROUP" --query fullyQualifiedDomainName -o tsv)
POSTGRES_CONNSTR="Host=$POSTGRES_FQDN;Port=5432;Database=$POSTGRES_DB;Username=$POSTGRES_ADMIN_USER;Password=$POSTGRES_ADMIN_PASSWORD;SslMode=Require;Trust Server Certificate=true"
info "Postgres FQDN: $POSTGRES_FQDN"

# ---------------------------------------------------------------------------
# App Service Plan (Linux)
# ---------------------------------------------------------------------------
info "Creating App Service Plan: $APP_PLAN"
az appservice plan create \
    --resource-group "$RESOURCE_GROUP" \
    --name "$APP_PLAN" \
    --location "$LOCATION" \
    --sku "$SKU" \
    --is-linux \
    --output none
ok "App Service Plan created"

# ---------------------------------------------------------------------------
# Web App - Fenicia.Auth (Container)
# ---------------------------------------------------------------------------
info "Creating Web App: $AUTH_APP"
az webapp create \
    --resource-group "$RESOURCE_GROUP" \
    --plan "$APP_PLAN" \
    --name "$AUTH_APP" \
    --deployment-container-image-name "nginx:latest" \
    --output none

# Configure container registry and startup
az webapp config container set \
    --resource-group "$RESOURCE_GROUP" \
    --name "$AUTH_APP" \
    --docker-custom-image-name "$ACR_LOGIN_SERVER/fenicia-auth:latest" \
    --docker-registry-server-url "https://$ACR_LOGIN_SERVER" \
    --docker-registry-server-user "$ACR_USERNAME" \
    --docker-registry-server-password "$ACR_PASSWORD" \
    --output none

# App settings
az webapp config appsettings set \
    --resource-group "$RESOURCE_GROUP" \
    --name "$AUTH_APP" \
    --settings \
        "DOTNET_ENVIRONMENT=Production" \
        "ASPNETCORE_URLS=http://+:8080" \
        "ConnectionStrings__Auth=$POSTGRES_CONNSTR" \
    --output none

ok "Auth API App created and configured"
AUTH_APP_URL=$(az webapp show --resource-group "$RESOURCE_GROUP" --name "$AUTH_APP" --query defaultHostName -o tsv)
info "Auth API URL: https://$AUTH_APP_URL"

# ---------------------------------------------------------------------------
# Web App - Fenicia.Module.Basic (Container)
# ---------------------------------------------------------------------------
info "Creating Web App: $BASIC_APP"
az webapp create \
    --resource-group "$RESOURCE_GROUP" \
    --plan "$APP_PLAN" \
    --name "$BASIC_APP" \
    --deployment-container-image-name "nginx:latest" \
    --output none

az webapp config container set \
    --resource-group "$RESOURCE_GROUP" \
    --name "$BASIC_APP" \
    --docker-custom-image-name "$ACR_LOGIN_SERVER/fenicia-module-basic:latest" \
    --docker-registry-server-url "https://$ACR_LOGIN_SERVER" \
    --docker-registry-server-user "$ACR_USERNAME" \
    --docker-registry-server-password "$ACR_PASSWORD" \
    --output none

az webapp config appsettings set \
    --resource-group "$RESOURCE_GROUP" \
    --name "$BASIC_APP" \
    --settings \
        "DOTNET_ENVIRONMENT=Production" \
        "ASPNETCORE_URLS=http://+:8080" \
        "ConnectionStrings__Auth=$POSTGRES_CONNSTR" \
    --output none

ok "Basic Module API App created and configured"
BASIC_APP_URL=$(az webapp show --resource-group "$RESOURCE_GROUP" --name "$BASIC_APP" --query defaultHostName -o tsv)
info "Basic API URL: https://$BASIC_APP_URL"

# ---------------------------------------------------------------------------
# Static Web App - Frontend (RECOMMENDED)
# ---------------------------------------------------------------------------
info "Creating Static Web App for frontend: $STATIC_WEB_APP"
az staticwebapp create \
    --resource-group "$RESOURCE_GROUP" \
    --name "$STATIC_WEB_APP" \
    --location "$LOCATION" \
    --source "" \
    --branch "" \
    --output none || info "Static Web App may already exist or manual deployment required"

SWA_HOSTNAME=$(az staticwebapp show --name "$STATIC_WEB_APP" --resource-group "$RESOURCE_GROUP" --query defaultHostname -o tsv 2>/dev/null || echo "")
if [[ -n "$SWA_HOSTNAME" ]]; then
    info "Static Web App hostname: $SWA_HOSTNAME"
fi

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------
echo ""
echo "=============================================================================="
echo "  Azure Resources Provisioned Successfully"
echo "=============================================================================="
echo ""
echo "Resource Group:     $RESOURCE_GROUP"
echo "Location:           $LOCATION"
echo ""
echo "Container Registry: $ACR_LOGIN_SERVER"
echo "Registry Username:  $ACR_USERNAME"
echo "Registry Password:  $ACR_PASSWORD"
echo ""
echo "PostgreSQL FQDN:    $POSTGRES_FQDN"
echo "Postgres ConnStr:   $POSTGRES_CONNSTR"
echo ""
echo "Auth API:           https://$AUTH_APP_URL"
echo "Basic API:          https://$BASIC_APP_URL"
echo "Static Web App:     $SWA_HOSTNAME"
echo ""
echo "=============================================================================="
echo "  Next Steps"
echo "=============================================================================="
echo ""
echo "1. CREATE GITHUB SECRETS:"
echo "   AZURE_CREDENTIALS              -> Service Principal JSON"
echo "   AZURE_REGISTRY_USERNAME        -> $ACR_USERNAME"
echo "   AZURE_REGISTRY_PASSWORD        -> $ACR_PASSWORD"
echo "   AZURE_RESOURCE_GROUP           -> $RESOURCE_GROUP"
echo "   AZURE_POSTGRES_CONNECTION_STRING -> $POSTGRES_CONNSTR"
echo "   AZURE_AUTH_API_URL             -> https://$AUTH_APP_URL"
echo "   AZURE_BASIC_API_URL            -> https://$BASIC_APP_URL"
echo "   AZURE_STATIC_WEB_APPS_API_TOKEN -> (get from Azure portal after SWA creation)"
echo ""
echo "2. PUSH IMAGES TO ACR:"
echo "   az acr login --name $CONTAINER_REGISTRY"
echo "   docker build -t $ACR_LOGIN_SERVER/fenicia-auth:latest -f Fenicia.Auth/Dockerfile ."
echo "   docker push $ACR_LOGIN_SERVER/fenicia-auth:latest"
echo "   docker build -t $ACR_LOGIN_SERVER/fenicia-module-basic:latest -f Fenicia.Module.Basic/Dockerfile ."
echo "   docker push $ACR_LOGIN_SERVER/fenicia-module-basic:latest"
echo ""
echo "3. CONFIGURE CORS on Auth & Basic APIs to allow your frontend URL."
echo ""
echo "4. FOR DEPLOYMENT TOKEN (Static Web Apps):"
echo "   az staticwebapp show --name $STATIC_WEB_APP --resource-group $RESOURCE_GROUP --query apiKey"
echo ""
echo "5. PUSH to main/develop branch to trigger GitHub Actions dep
