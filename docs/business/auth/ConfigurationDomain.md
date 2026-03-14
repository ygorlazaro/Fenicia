# Configuration Domain

The Configuration domain manages user and company settings within the Fenicia authentication system. Configurations store user preferences like language and timezone, which can be scoped to either the user level or a specific company.

## Overview

This domain provides functionality for:
- Retrieving configurations for a user (optionally filtered by company)
- Creating or updating configuration entries (upsert pattern)

## Business Logic

### Configuration Entity
A configuration represents a user or company setting with the following characteristics:
- **Id**: Unique identifier (GUID)
- **UserId**: User who owns this configuration (required)
- **CompanyId**: Company scope (optional, for company-specific settings)
- **ConfigType**: Type of configuration (e.g., Language, Timezone)
- **Value**: The configuration value (string)

### Configuration Types
The system supports the following configuration types:
- **Language**: User's preferred language (e.g., "pt-BR", "en-US")
- **Timezone**: User's preferred timezone

### Configuration Retrieval
- Results are filtered by UserId and optionally by CompanyId
- Results are ordered alphabetically by ConfigType
- Returns all matching configurations in a list

### Upsert Pattern
The domain uses an upsert pattern for configuration management:
- If a configuration with the same UserId, CompanyId, and ConfigType exists, it is updated
- If it doesn't exist, a new configuration is created
- This ensures each user can have only one configuration value per type per company

## Components

### Controllers

#### ConfigurationController
HTTP endpoint controller providing REST API for configuration operations.

- `GET /Configuration` - Retrieves configurations for the logged-in user (optional company filter)
- `PATCH /Configuration/{id}` - Creates or updates a configuration entry

### Handlers

#### GetConfigurationHandler
Handles retrieval of user configurations. Returns configurations filtered by user and optionally by company, ordered by ConfigType.

#### UpsertConfigurationHandler
Handles configuration upsert requests. Creates new configuration if it doesn't exist, or updates the existing value if it does.

### Commands

#### UpsertConfigurationCommand
Command record for creating or updating a configuration.
- `Id`: Configuration ID (optional, used for routing)
- `UserId`: User ID who owns this configuration
- `ConfigType`: Type of configuration (Language, Timezone)
- `Value`: Configuration value
- `CompanyId`: Company ID (for company-scoped configurations)

### Queries

#### GetConfigurationQuery
Query for retrieving user configurations.
- `UserId`: ID of the user
- `CompanyId`: Optional company ID to filter by

### Responses

#### GetConfigurationResponse
Response model containing configuration information.
- `Id`: Configuration ID
- `UserId`: User ID
- `CompanyId`: Company ID (nullable)
- `ConfigType`: Configuration type
- `Value`: Configuration value

## Data Model

### ConfigurationModel
Entity representing a configuration in the database:
- Mapped to `auth.Configuration` table
- Inherits from `BaseCompanyModel` (includes Id, CreatedAt, UpdatedAt)
- Has relationships with: User, Company

### ConfigType Enum
Enumeration of supported configuration types:
- `Language = 1`: User's preferred language
- `Timezone = 2`: User's preferred timezone

## Security

- All configuration endpoints require authentication
- Users can only access their own configurations
- Company-scoped configurations are filtered by the provided CompanyId

## Testing

The Configuration domain has comprehensive unit tests located in `Fenicia.Auth.Tests/Domains/Configuration/`.

### Test Coverage

#### ConfigurationControllerTests
Tests the HTTP endpoint layer including:
- Retrieving configurations for authenticated user
- Creating new configurations via upsert
- Updating existing configurations
- Company-scoped configuration handling
- WideEventContext propagation for request tracking
- Controller attribute validation (Authorize, Route, Produces)

**Key test scenarios:**
- Empty configuration list returns empty list
- User with configurations returns all matching
- Company ID filter returns only company configurations
- Non-existent configuration creates new entry
- Existing configuration updates value
- WideEventContext is properly set

#### GetConfigurationHandlerTests
Tests configuration retrieval logic including:
- User-company filtering
- ConfigType ordering
- Response data mapping

**Key test scenarios:**
- User with no configurations returns empty list
- User with configurations returns all matching
- Company ID filter works correctly
- Non-existent company returns empty list
- Results ordered by ConfigType
- Response contains all correct data fields

#### UpsertConfigurationHandlerTests
Tests the upsert logic including:
- Create new configuration
- Update existing configuration
- Multiple configurations per user/type/company
- Data integrity (ID preservation)
- Value handling (empty strings, long values)

**Key test scenarios:**
- Non-existent configuration creates new entry
- Existing configuration updates value
- Company-scoped configuration created correctly
- Different ConfigTypes create separate entries
- Update preserves original ID
- Multiple updates only keep last value
- Empty value can be saved
- Long text values saved successfully
