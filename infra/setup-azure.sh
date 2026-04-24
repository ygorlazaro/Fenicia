#!/usr/bin/env bash
#
# =============================================================================
# Fenicia Platform - Azure Infrastructure Setup Script
# =============================================================================
# This script interactively creates ALL required Azure resources for deploying
# the Fenicia platform (Frontend + Fenicia.Auth + Fenicia.Module.Basic).
#
# Usage:
#   chmod +x infra/setup-azure.sh
#   ./infra/setup-azure.sh
#
# The script will:
#   1. Check if Azure CLI is installed
#   2. Prompt for az login if not authenticated
#   3. Ask for configuration parameters
#   4. Create the Resource Group and all resources
#   5. Output GitHub secrets to configure
# =============================================================================

set -euo pipefail

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# =============================================================================
# Helper Functions
# =============================================================================

print_header() {
    echo ""
    echo -e "${BLUE}══════════════════════════════════════════════════════════════════════${NC}"
    echo -e "${BLUE}  $1${NC}"
    echo -e "${BLUE}══════════════════════════════════════════════════════════════════════${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${CYAN}ℹ $1${NC}"
}

ask_input() {
    local prompt="$1"
    local default="$2"
    local result

    if [ -n "$default" ]; then
        read -rp "$(echo -e "${YELLOW}$prompt${NC} [${default}]: ")" result
        echo "${result:-$default}"
    else
        read -rp "$(echo -e "${YELLOW}$prompt${NC}: ")" result
        echo "$result"
    fi
}

ask_password() {
    local prompt="$1"
    local result

    read -rsp "$(echo -e "${YELLOW}$prompt${NC}: ")" result
    echo ""
    echo "$result"
}

confirm() {
    local prompt="$1"
    local result

    read -rp "$(echo -e "${YELLOW}$prompt${NC} [Y/n]: ")" result
    case "${result,,}" in
        y|yes|"") return 0 ;;
        *) return 1 ;;
    esac
}

# =============================================================================
# Azure CLI Check
# =============================================================================

check_az_cli() {
    print_header "Checking Prerequisites"

    if ! command -v az &> /dev/null; then
        print_error "Azure CLI is not installed."
        echo ""
        echo "Please install Azure CLI:"
        echo "  - macOS: brew install azure-cli"
        echo "  - Ubuntu/Debian: curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash"
        echo "  - Windows: winget install Microsoft.AzureCLI"
        echo ""
        echo "Visit: https://docs.microsoft.com/cli/azure/install-azure-cli"
        exit 1
    fi
    print_success "Azure CLI is installed ($(az version --query '\"azure-cli\"' -o tsv))"

    if ! az account show &> /dev/null; then
        print_warning "You are not logged in to Azure."
        echo ""
        if confirm "Run 'az login' now?"; then
            az login
            if ! az account show &> /dev/null; then
                print_error "Login failed. Please try again."
                exit 1
            fi
            print_success "Logged in successfully!"
        else
            print_error "Azure login is required. Exiting."
            exit 1
        fi
    else
        local sub_name
        sub_name=$(az account show --query name -o tsv)
        print_success "Already logged in as: $sub_name"
    fi

    # Show available subscriptions and let user choose
    local subs_count
    subs_count=$(az account list --query 'length([])' -o tsv)

    if [ "$subs_count" -gt 1 ]; then
        echo ""
        print_info "You have multiple subscriptions:"
        az account list --query '[].{Name:name, ID:id, Default:isDefault}' -o table
        echo ""
        local current_sub
        current_sub=$(az account show --query id -o tsv)
        local chosen_sub
        chosen_sub=$(ask_input "Enter the subscription ID to use (or press Enter for current)" "$current_sub")
        az account set --subscription "$chosen_sub"
        print_success "Using subscription: $(az account show --query name -o tsv)"
    fi
}

# =============================================================================
# Configuration
# =============================================================================

collect_config() {
    print_header "Configuration"

    echo "Please provide the following configuration values:"
    echo "(Press Enter to accept defaults where shown)"
    echo ""

    RESOURCE_GROUP=$(ask_input "Resource Group name" "rg-fenicia-prod")
    LOCATION=$(ask_input "Azure Region" "eastus")
    ACR_NAME=$(ask_input "Container Registry name (must be globally unique)" "feniciacr$(date +%s | tail -c 5)")
    POSTGRES_SERVER_NAME=$(ask_input "PostgreSQL server name (must be globally unique)" "fenicia-postgres-$(date +%s | tail -c 5)")
    POSTGRES_ADMIN_USER=$(ask_input "PostgreSQL admin username" "feniciaadmin")
    POSTGRES_PASSWORD=$(ask_password "PostgreSQL admin password (min 8 chars)")
    echo ""

    while [ ${#POSTGRES_PASSWORD} -lt 8 ]; do
        print_error "Password must be at least 8 characters."
        POSTGRES_PASSWORD=$(ask_password "PostgreSQL admin password")
        echo ""
    done

    AUTH_APP_NAME=$(ask_input "Auth API App Service name (must be globally unique)" "fenicia-auth-api-$(date +%s | tail -c 5)")
    BASIC_APP_NAME=$(ask_input "Basic API App Service name (must be globally unique)" "fenicia-basic-api-$(date +%s | tail -c 5)")
    STATIC_WEB_APP_NAME=$(ask_input "Static Web App name (must be globally unique)" "fenicia-web-$(date +%s | tail -c 5)")
    APP_SERVICE_PLAN_NAME=$(ask_input "App Service Plan name" "asp-fenicia-prod")

    echo ""
    print_info "Configuration Summary:"
    echo "  Resource Group:      $RESOURCE_GROUP"
    echo "  Location:            $LOCATION"
    echo "  Container Registry:  $ACR_NAME.azurecr.io"
    echo "  PostgreSQL Server:   $POSTGRES_SERVER_NAME.postgres.database.azure.com"
    echo "  Auth API:            $AUTH_APP_NAME.azurewebsites.net"
    echo "  Basic API:           $BASIC_APP_NAME.azurewebsites.net"
    echo "  Static Web App:      $STATIC_WEB_APP_NAME"
    echo ""

    if ! confirm "Proceed with creating these resources?"; then
        print_error "Aborted by user."
        exit 1
    fi
}

# =============================================================================
# Resource Creation
# =============================================================================

create_resource_group() {
    print_header "Step 1: Creating Resource Group"

    if az group show --name "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "Resource group '$RESOURCE_GROUP' already exists."
    else
        az group create \
            --name "$RESOURCE_GROUP" \
            --location "$LOCATION" \
            --tags \
                Project=Fenicia \
                Environment=Production \
                ManagedBy=Script
        print_success "Resource group '$RESOURCE_GROUP' created in '$LOCATION'"
    fi
}

create_acr() {
    print_header "Step 2: Creating Azure Container Registry"

    if az acr show --name "$ACR_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "Container Registry '$ACR_NAME' already exists."
    else
        az acr create \
            --resource-group "$RESOURCE_GROUP" \
            --name "$ACR_NAME" \
            --sku Basic \
            --admin-enabled true \
            --location "$LOCATION"
        print_success "Container Registry '$ACR_NAME.azurecr.io' created"
    fi

    # Get credentials
    ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query username -o tsv)
    ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query 'passwords[0].value' -o tsv)
    print_success "ACR credentials retrieved"
}

create_postgres() {
    print_header "Step 3: Creating Azure Database for PostgreSQL"

    if az postgres flexible-server show --name "$POSTGRES_SERVER_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "PostgreSQL server '$POSTGRES_SERVER_NAME' already exists."
    else
        print_info "Creating PostgreSQL Flexible Server (this may take 5-10 minutes)..."
        az postgres flexible-server create \
            --resource-group "$RESOURCE_GROUP" \
            --name "$POSTGRES_SERVER_NAME" \
            --location "$LOCATION" \
            --admin-user "$POSTGRES_ADMIN_USER" \
            --admin-password "$POSTGRES_PASSWORD" \
            --sku-name Standard_B1ms \
            --tier Burstable \
            --storage-size 32 \
            --version 15 \
            --public-access 0.0.0.0 \
            --database-name fenicia \
            --yes

        print_success "PostgreSQL server '$POSTGRES_SERVER_NAME' created"
    fi

    # Allow Azure services
    print_info "Configuring firewall rules..."
    az postgres flexible-server firewall-rule create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$POSTGRES_SERVER_NAME" \
        --rule-name AllowAzureServices \
        --start-ip-address 0.0.0.0 \
        --end-ip-address 0.0.0.0 \
        --output none

    print_success "Firewall configured to allow Azure services"

    # Build connection string
    POSTGRES_CONNECTION_STRING="Host=$POSTGRES_SERVER_NAME.postgres.database.azure.com;Port=5432;Database=fenicia;Username=$POSTGRES_ADMIN_USER;Password=$POSTGRES_PASSWORD;SslMode=Require"
}

create_app_service_plan() {
    print_header "Step 4: Creating App Service Plan"

    if az appservice plan show --name "$APP_SERVICE_PLAN_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "App Service Plan '$APP_SERVICE_PLAN_NAME' already exists."
    else
        az appservice plan create \
            --resource-group "$RESOURCE_GROUP" \
            --name "$APP_SERVICE_PLAN_NAME" \
            --location "$LOCATION" \
            --sku B1 \
            --is-linux \
            --tags \
                Project=Fenicia \
                Environment=Production
        print_success "App Service Plan '$APP_SERVICE_PLAN_NAME' created (B1 Linux)"
    fi
}

create_web_apps() {
    print_header "Step 5: Creating Web Apps for Backend Services"

    # Auth Web App
    if az webapp show --name "$AUTH_APP_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "Web App '$AUTH_APP_NAME' already exists."
    else
        az webapp create \
            --resource-group "$RESOURCE_GROUP" \
            --plan "$APP_SERVICE_PLAN_NAME" \
            --name "$AUTH_APP_NAME" \
            --deployment-container-image-name "$ACR_NAME.azurecr.io/fenicia-auth:latest"
        print_success "Web App '$AUTH_APP_NAME' created"
    fi

    # Configure Auth Web App
    print_info "Configuring Auth Web App settings..."
    az webapp config appsettings set \
        --resource-group "$RESOURCE_GROUP" \
        --name "$AUTH_APP_NAME" \
        --settings \
            "ConnectionStrings__Auth=$POSTGRES_CONNECTION_STRING" \
            "DOTNET_ENVIRONMENT=Production" \
            "ASPNETCORE_URLS=http://+:8080" \
        --output none
    print_success "Auth Web App configured"

    # Enable ACR pull
    print_info "Granting ACR pull access to Auth Web App..."
    local auth_principal_id
    auth_principal_id=$(az webapp identity assign --name "$AUTH_APP_NAME" --resource-group "$RESOURCE_GROUP" --query principalId -o tsv)
    az role assignment create \
        --assignee "$auth_principal_id" \
        --role AcrPull \
        --scope "$(az acr show --name "$ACR_NAME" --resource-group "$RESOURCE_GROUP" --query id -o tsv)" \
        --output none
    print_success "Auth Web App can pull from ACR"

    # Basic Web App
    if az webapp show --name "$BASIC_APP_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "Web App '$BASIC_APP_NAME' already exists."
    else
        az webapp create \
            --resource-group "$RESOURCE_GROUP" \
            --plan "$APP_SERVICE_PLAN_NAME" \
            --name "$BASIC_APP_NAME" \
            --deployment-container-image-name "$ACR_NAME.azurecr.io/fenicia-module-basic:latest"
        print_success "Web App '$BASIC_APP_NAME' created"
    fi

    # Configure Basic Web App
    print_info "Configuring Basic Web App settings..."
    az webapp config appsettings set \
        --resource-group "$RESOURCE_GROUP" \
        --name "$BASIC_APP_NAME" \
        --settings \
            "ConnectionStrings__Auth=$POSTGRES_CONNECTION_STRING" \
            "DOTNET_ENVIRONMENT=Production" \
            "ASPNETCORE_URLS=http://+:8080" \
        --output none
    print_success "Basic Web App configured"

    # Enable ACR pull for Basic
    print_info "Granting ACR pull access to Basic Web App..."
    local basic_principal_id
    basic_principal_id=$(az webapp identity assign --name "$BASIC_APP_NAME" --resource-group "$RESOURCE_GROUP" --query principalId -o tsv)
    az role assignment create \
        --assignee "$basic_principal_id" \
        --role AcrPull \
        --scope "$(az acr show --name "$ACR_NAME" --resource-group "$RESOURCE_GROUP" --query id -o tsv)" \
        --output none
    print_success "Basic Web App can pull from ACR"
}

create_static_web_app() {
    print_header "Step 6: Creating Static Web App for Frontend"

    if az staticwebapp show --name "$STATIC_WEB_APP_NAME" --resource-group "$RESOURCE_GROUP" &> /dev/null; then
        print_warning "Static Web App '$STATIC_WEB_APP_NAME' already exists."
    else
        print_info "Creating Static Web App..."
        az staticwebapp create \
            --name "$STATIC_WEB_APP_NAME" \
            --resource-group "$RESOURCE_GROUP" \
            --location "$LOCATION" \
            --sku Free
        print_success "Static Web App '$STATIC_WEB_APP_NAME' created"
    fi

    # Get deployment token
    STATIC_WEB_APP_TOKEN=$(az staticwebapp secrets list --name "$STATIC_WEB_APP_NAME" --resource-group "$RESOURCE_GROUP" --query properties.apiKey -o tsv)
    STATIC_WEB_APP_URL="https://$(az staticwebapp show --name "$STATIC_WEB_APP_NAME" --resource-group "$RESOURCE_GROUP" --query defaultHostname -o tsv)"
    print_success "Static Web App URL: $STATIC_WEB_APP_URL"
}

create_service_principal() {
    print_header "Step 7: Creating Service Principal for GitHub Actions"

    print_info "Creating Service Principal with Contributor role on Resource Group..."

    local sub_id
    sub_id=$(az account show --query id -o tsv)
    local scope="/subscriptions/$sub_id/resourceGroups/$RESOURCE_GROUP"

    # Delete existing SP with same name if exists
    local existing_sp
    existing_sp=$(az ad sp list --display-name "fenicia-github-actions-$RESOURCE_GROUP" --query '[].appId' -o tsv)
    if [ -n "$existing_sp" ]; then
        print_warning "Existing Service Principal found. Deleting..."
        az ad sp delete --id "$existing_sp" --output none || true
        sleep 5
    fi

    AZURE_CREDENTIALS=$(az ad sp create-for-rbac \
        --name "fenicia-github-actions-$RESOURCE_GROUP" \
        --role contributor \
        --scopes "$scope" \
        --sdk-auth)

    print_success "Service Principal created"
}

# =============================================================================
# Summary & Next Steps
# =============================================================================

print_summary() {
    print_header "Deployment Complete! 🎉"

    echo ""
    echo -e "${GREEN}All Azure resources have been created successfully.${NC}"
    echo ""

    echo -e "${CYAN}══════════════════════════════════════════════════════════════════════${NC}"
    echo -e "${CYAN}  Resource URLs${NC}"
    echo -e "${CYAN}══════════════════════════════════════════════════════════════════════${NC}"
    echo "  Auth API:     https://$AUTH_APP_NAME.azurewebsites.net"
    echo "  Basic API:    https://$BASIC_APP_NAME.azurewebsites.net"
    echo "  Frontend:     $STATIC_WEB_APP_URL"
    echo "  ACR Login:    $ACR_NAME.azurecr.io"
    echo ""

    echo -e "${CYAN}══════════════════════════════════════════════════════════════════════${NC}"
    echo -e "${CYAN}  GitHub Secrets to Configure${NC}"
    echo -e "${CYAN}══════════════════════════════════════════════════════════════════════${NC}"
    echo ""
    echo "Go to: GitHub Repo > Settings > Secrets and variables > Actions"
    echo ""
    echo "Add the following secrets:"
    echo ""
    echo -e "${YELLOW}AZURE_CREDENTIALS${NC}"
    echo "$AZURE_CREDENTIALS"
    echo ""
    echo -e "${YELLOW}AZURE_REGISTRY_USERNAME${NC}"
    echo "$ACR_USERNAME"
    echo ""
    echo -e "${YELLOW}AZURE_REGISTRY_PASSWORD${NC}"
    echo "$ACR_PASSWORD"
    echo ""
    echo -e "${YELLOW}AZURE_RESOURCE_GROUP${NC}"
    echo "$RESOURCE_GROUP"
    echo ""
    echo -e "${YELLOW}AZURE_POSTGRES_CONNECTION_STRING${NC}"
    echo "$POSTGRES_CONNECTION_STRING"
    echo ""
    echo -e "${YELLOW}AZURE_AUTH_API_URL${NC}"
    echo "https://$AUTH_APP_NAME.azurewebsites.net"
    echo ""
    echo -e "${YELLOW}AZURE_BASIC_API_URL${NC}"
    echo "https://$BASIC_APP_NAME.azurewebsites.net"
    echo ""
    echo -e "${YELLOW}AZURE_PROJECTS_API_URL${NC}"
    echo "https://fenicia-projects-api.azurewebsites.net  (update when deployed)"
    echo ""
    echo -e "${YELLOW}AZURE_STATIC_WEB_APPS_API_TOKEN${NC}"
    echo "$STATIC_WEB_APP_TOKEN"
    echo ""

    echo -e "${CYAN}══════════════════════════════════════════════════════════════════════${NC}"
    echo -e "${CYAN}  Next Steps${NC}"
    echo -e "${CYAN}══════════════════════════════════════════════════════════════════════${NC}"
    echo ""
    echo "1. Copy the GitHub secrets above into your repository settings."
    echo ""
    echo "2. Build and push Docker images:"
    echo "   docker build -t $ACR_NAME.azurecr.io/fenicia-auth:latest -f Fenicia.Auth/Dockerfile ."
    echo "   docker build -t $ACR_NAME.azurecr.io/fenicia-module-basic:latest -f Fenicia.Module.Basic/Dockerfile ."
    echo "   az acr login --name $ACR_NAME"
    echo "   docker push $ACR_NAME.azurecr.io/fenicia-auth:latest"
    echo "   docker push $ACR_NAME.azurecr.io/fenicia-module-basic:latest"
    echo ""
    echo "3. Push code to GitHub to trigger CI/CD pipelines."
    echo ""
    echo "4. Monitor deployments:"
    echo "   az webapp log tail --name $AUTH_APP_NAME --resource-group $RESOURCE_GROUP"
    echo "   az webapp log tail --name $BASIC_APP_NAME --resource-group $RESOURCE_GROUP"
    echo ""

    # Save to file for reference
    local output_file="azure-deployment-$(date +%Y%m%d-%H%M%S).json"
    cat > "$output_file" <<EOF
{
  "resourceGroup": "$RESOURCE_GROUP",
  "location": "$LOCATION",
  "containerRegistry": {
    "name": "$ACR_NAME",
    "loginServer": "$ACR_NAME.azurecr.io",
    "username": "$ACR_USERNAME",
    "password": "$ACR_PASSWORD"
  },
  "database": {
    "serverName": "$POSTGRES_SERVER_NAME",
    "adminUser": "$POSTGRES_ADMIN_USER",
    "connectionString": "$POSTGRES_CONNECTION_STRING"
  },
  "authApi": {
    "name": "$AUTH_APP_NAME",
    "url": "https://$AUTH_APP_NAME.azurewebsites.net"
  },
  "basicApi": {
    "name": "$BASIC_APP_NAME",
    "url": "https://$BASIC_APP_NAME.azurewebsites.net"
  },
  "frontend": {
    "name": "$STATIC_WEB_APP_NAME",
    "url": "$STATIC_WEB_APP_URL",
    "deploymentToken": "$STATIC_WEB_APP_TOKEN"
  },
  "githubSecrets": {
    "AZURE_CREDENTIALS": $AZURE_CREDENTIALS,
    "AZURE_REGISTRY_USERNAME": "$ACR_USERNAME",
    "AZURE_REGISTRY_PASSWORD": "$ACR_PASSWORD",
    "AZURE_RESOURCE_GROUP": "$RESOURCE_GROUP",
    "AZURE_POSTGRES_CONNECTION_STRING": "$POSTGRES_CONNECTION_STRING",
    "AZURE_AUTH_API_URL": "https://$AUTH_APP_NAME.azurewebsites.net",
    "AZURE_BASIC_API_URL": "https://$BASIC_APP_NAME.azurewebsites.net",
    "AZURE_STATIC_WEB_APPS_API_TOKEN": "$STATIC_WEB_APP_TOKEN"
  }
}
EOF
    print_success "Deployment details saved to: $output_file"
    echo ""

    if confirm "Would you like to see a summary of all created Azure resources?"; then
        echo ""
        az resource list --resource-group "$RESOURCE_GROUP" --query '[].{Name:name, Type:type, Location:location}' -o table
    fi
}

# =============================================================================
# Main
# =============================================================================

main() {
    print_header "Fenicia Platform - Azure Infrastructure Setup"
    echo "This script will create all Azure resources needed to deploy"
    echo "the Fenicia platform (Frontend + Auth API + Basic API)."
    echo ""

    if ! confirm "Continue?"; then
        print_error "Aborted by user."
        exit 0
    fi

    check_az_cli
    collect_config
    create_resource_group
    create_acr
    create_postgres
    create_app_service_plan
    create_web_apps
    create_static_web_app
    create_service_principal
    print_summary
}

main "$@"
