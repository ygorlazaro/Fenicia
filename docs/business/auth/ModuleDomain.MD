# Module Domain

## Overview

The Module domain manages system modules that can be subscribed to by companies. It provides functionality to:
1. List available modules publicly (for marketing/selection pages)
2. Retrieve modules available to a specific user within a company based on their subscriptions

## Architecture

### Controllers

#### 1. ModuleController
**Location:** `Fenicia.Auth/Domains/Module/ModuleController.cs`

HTTP endpoint for module operations.

**Endpoints:**
- `GET /module` - Retrieves available modules with pagination (publicly accessible)

**Key Features:**
- Publicly accessible via `[AllowAnonymous]`
- Sets `WideEventContext.UserId` to "Guest" for tracking
- Excludes Auth type modules from results

### Handlers

#### 1. GetModulesHandler
**Location:** `Fenicia.Auth/Domains/Module/Handlers/GetModulesHandler.cs`

Handler for retrieving a paginated list of available modules.

**Key Methods:**
- `Handle(GetModulesQuery query, CancellationToken ct)` - Returns paginated modules excluding Auth type

**Behavior:**
- Excludes modules with type `ModuleType.Auth` (internal authentication module)
- Orders results by `ModuleType` ascending
- Returns pagination metadata (total count, current page, items per page, total pages)

#### 2. GetUserModuleHandler
**Location:** `Fenicia.Auth/Domains/Module/Handlers/GetUserModuleHandler.cs`

Handler for retrieving modules available to a specific user within a company.

**Key Methods:**
- `Handle(GetUserModulesQuery query, CancellationToken ct)` - Returns user's accessible modules

**Behavior:**
- Filters by user role in the company
- Only returns modules from active subscriptions
- Only returns modules with active subscription credits
- Checks subscription date ranges (current date must be within start and end dates)
- Returns distinct results to avoid duplicates

### Queries

#### 1. GetModulesQuery
**Location:** `Fenicia.Auth/Domains/Module/Queries/GetModulesQuery.cs`

Query for paginated module retrieval.

**Properties:**
- `Page` (int, default: 1) - Page number
- `PerPage` (int, default: 20) - Items per page

#### 2. GetUserModulesQuery
**Location:** `Fenicia.Auth/Domains/Module/Queries/GetUserModulesQuery.cs`

Query for user-specific module access.

**Properties:**
- `CompanyId` (Guid) - Company identifier
- `UserId` (Guid) - User identifier

### Responses

#### 1. GetModuleResponse
**Location:** `Fenicia.Auth/Domains/Module/Responses/GetModuleResponse.cs`

Response DTO for public module listings.

**Properties:**
- `Id` (Guid) - Module unique identifier
- `Name` (string) - Module display name
- `Type` (ModuleType) - Module type enumeration

#### 2. GetUserModulesResponse
**Location:** `Fenicia.Auth/Domains/Module/Responses/GetUserModulesResponse.cs`

Response DTO for user-specific module access.

**Properties:**
- `Id` (Guid) - Module unique identifier
- `Name` (string) - Module display name
- `Type` (ModuleType) - Module type enumeration

## Usage Flow

1. **Public Module Listing:**
   - Client calls `GET /module?page=1&perPage=20`
   - Returns paginated list of available modules (excluding Auth type)
   - Used for marketing pages or module selection during registration

2. **User-Specific Modules:**
   - Client requests modules available to them in a company
   - Handler checks user's role, company's active subscriptions, and subscription credits
   - Returns only modules the user can actually access

## Module Types

The system supports various module types (defined in `ModuleType` enum):
- `Auth` - Internal authentication module (excluded from public listings)
- `Basic` - Basic functionality module
- `SocialNetwork` - Social networking features
- `Hr` - Human resources module
- And other specialized types...

## Unit Tests

### GetModulesHandlerTests
**Location:** `Fenicia.Auth.Tests/Domains/Module/GetModulesHandlerTests.cs`

Tests cover:
- Pagination returns correct data and metadata
- Auth type modules are excluded from results
- Results are ordered by module type
- Empty results are handled correctly
- Response contains all required fields

**Test Methods:**
- `Handle_WhenModulesExist_ReturnsPaginatedModules`
- `Handle_WhenModulesExist_ExcludesErpAndAuthTypes`
- `Handle_WhenPaginationIsApplied_ReturnsCorrectPage`
- `Handle_WhenNoModulesExist_ReturnsEmptyPagination`
- `Handle_WhenPageExceedsTotalPages_ReturnsEmptyData`
- `Handle_ResultsAreOrderedByType`
- `Handle_WithDefaultRequest_ReturnsFirstPage`
- `Handle_VerifiesResponseContainsAllFields`

### ModuleControllerTests
**Location:** `Fenicia.Auth.Tests/Domains/Module/ModuleControllerTests.cs`

Tests cover:
- Returns correct pagination responses
- Excludes Auth type modules
- Sets WideEventContext UserId to "Guest"
- Controller has correct attributes (Authorize, Route, Produces)
- Endpoint has AllowAnonymous attribute

**Test Methods:**
- `GetAllModulesAsync_WhenNoModulesExist_ReturnsOkWithEmptyPagination`
- `GetAllModulesAsync_WhenModulesExist_ReturnsOkWithPagination`
- `GetAllModulesAsync_ExcludesErpAndAuthModuleTypes`
- `GetAllModulesAsync_SetsWideEventContextUserIdToGuest`
- `GetAllModulesAsync_WithPagination_ReturnsCorrectPage`
- `ModuleController_HasAuthorizeAttribute`
- `ModuleController_HasRouteAttribute`
- `ModuleController_HasProducesAttribute`
- `GetAllModulesAsync_HasAllowAnonymousAttribute`

## Subscription-Based Access Model

The Module domain integrates with the subscription system:

1. **Companies** subscribe to modules through **Subscriptions**
2. **Subscription Credits** define which modules are available under each subscription
3. **Users** access modules based on their **User Roles** in the company
4. Only active subscriptions with valid date ranges grant module access

This ensures proper access control based on paid subscriptions.
