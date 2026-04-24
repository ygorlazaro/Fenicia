// =============================================================================
// Fenicia Platform - Azure Infrastructure as Code (Bicep)
// =============================================================================
// Deploys:
//   - Azure Container Registry
//   - Azure Database for PostgreSQL Flexible Server
//   - Azure Static Web Apps (Frontend)
//   - Azure App Services (Linux Container) for .NET APIs
//   - Azure Application Insights
// =============================================================================

@description('Environment name (dev, staging, prod)')
param environmentName string = 'prod'

@description('Primary Azure region')
param location string = resourceGroup().location

@description('PostgreSQL admin username')
param postgresAdminUser string

@description('PostgreSQL admin password')
@secure()
param postgresAdminPassword string

@description('Enable high availability for PostgreSQL')
param postgresHighAvailability bool = false

// =============================================================================
// Naming conventions
// =============================================================================
var prefix = 'fenicia-${environmentName}'
var acrName = '${prefix}acr'
var postgresName = '${prefix}-postgres'
var staticWebAppName = '${prefix}-web'
var authAppName = '${prefix}-auth-api'
var basicAppName = '${prefix}-basic-api'
var appInsightsName = '${prefix}-insights'
var logAnalyticsName = '${prefix}-logs'

// =============================================================================
// Log Analytics Workspace (for Application Insights)
// =============================================================================
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// =============================================================================
// Application Insights
// =============================================================================
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// =============================================================================
// Azure Container Registry
// =============================================================================
resource acr 'Microsoft.ContainerRegistry/registries@2023-11-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

// =============================================================================
// Azure Database for PostgreSQL Flexible Server
// =============================================================================
resource postgres 'Microsoft.DBforPostgreSQL/flexibleServers@2023-12-01-preview' = {
  name: postgresName
  location: location
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    version: '16'
    administratorLogin: postgresAdminUser
    administratorLoginPassword: postgresAdminPassword
    storage: {
      storageSizeGB: 32
    }
    highAvailability: {
      mode: postgresHighAvailability ? 'ZoneRedundant' : 'Disabled'
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
  }
}

resource postgresDatabase 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-12-01-preview' = {
  parent: postgres
  name: 'fenicia'
}

resource postgresFirewallRule 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-12-01-preview' = {
  parent: postgres
  name: 'AllowAllAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

// =============================================================================
// App Service Plan (Shared by all .NET container apps)
// =============================================================================
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${prefix}-plan'
  location: location
  kind: 'linux'
  properties: {
    reserved: true
    perSiteScaling: false
  }
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
}

// =============================================================================
// Helper: App Service Module for .NET Containers
// =============================================================================
module apiAppService 'modules/container-app-service.bicep' = [for api in [
  {
    name: authAppName
    imageName: 'fenicia-auth'
  }
  {
    name: basicAppName
    imageName: 'fenicia-module-basic'
  }
]: {
  name: 'deploy-${api.name}'
  params: {
    appName: api.name
    location: location
    appServicePlanId: appServicePlan.id
    acrLoginServer: acr.properties.loginServer
    acrUsername: acr.listCredentials().username
    acrPassword: acr.listCredentials().passwords[0].value
    imageName: api.imageName
    imageTag: 'latest'
    appInsightsConnectionString: appInsights.properties.ConnectionString
    postgresConnectionString: 'Host=${postgres.properties.fullyQualifiedDomainName};Port=5432;Database=fenicia;Username=${postgresAdminUser};Password=${postgresAdminPassword}'
  }
}]

// =============================================================================
// Azure Static Web Apps (Frontend)
// =============================================================================
resource static
