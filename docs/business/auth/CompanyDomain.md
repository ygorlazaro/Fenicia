# Company Domain

The Company domain manages company entities within the Fenicia authentication system. Companies represent organizations that users can be associated with through roles.

## Overview

This domain provides functionality for:
- Retrieving companies associated with a user
- Updating company information
- Checking if a company exists by CNPJ

## Business Logic

### Company Entity
A company represents a business organization in the system with the following characteristics:
- **Id**: Unique identifier (GUID)
- **Name**: Company name (required, max 50 characters)
- **CNPJ**: Brazilian company tax identification number (14 characters, required)
- **IsActive**: Indicates whether the company is active (default: true)
- **AddressId**: Optional link to company address

### Company Retrieval
- Users can only see companies they are associated with through user roles
- Results are paginated and ordered alphabetically by company name
- Only active companies are returned
- Each returned company includes the user's role in that company

### Company Updates
- Only users with the "Admin" role can update company information
- The company must exist and be active to be updated
- Currently, only the company name can be modified

### CNPJ Validation
- CNPJ must be unique across all active companies
- Used during company registration to prevent duplicates

## Components

### Controllers

#### CompanyController
HTTP endpoint controller providing REST API for company operations.

- `GET /Company` - Retrieves paginated list of companies for the logged-in user
- `PATCH /Company/{id}` - Updates company information (Admin only)

### Handlers

#### GetCompaniesByUserHandler
Handles retrieval of companies associated with a user. Returns paginated results including company details and user's role.

#### UpdateCompanyHandler
Handles company update requests. Validates company existence, active status, and user permissions before applying changes.

#### CheckCompanyExistsHandler
Handles CNPJ existence checks. Used to verify if a CNPJ is already registered.

### Commands

#### UpdateCompanyCommand
Command record for updating company information.
- `CompanyId`: ID of the company to update
- `UserId`: ID of the user performing the update (for permission validation)
- `Name`: New company name

### Queries

#### GetCompaniesByUserQuery
Query for retrieving companies a user is associated with.
- `UserId`: ID of the user
- `Page`: Page number for pagination
- `PerPage`: Number of items per page

#### CheckCompanyExistsQuery
Query for checking CNPJ existence.
- `CNPJ`: CNPJ to check
- `OnlyActive`: Whether to consider only active companies

### Responses

#### GetCompaniesByUserResponse
Response model containing company information for a user.
- `Id`: Company ID
- `Name`: Company name
- `CNPJ`: Company CNPJ
- `Role`: User's role in the company

### Extensions

#### CompanyExtensions
Database extension methods for CompanyModel:
- `AnyCnpjAsync`: Checks if any company exists with the given CNPJ
- `AnyAsync`: Checks if any company exists with the given ID
- `AnyActiveAsync`: Checks if any active company exists with the given ID

## Data Model

### CompanyModel
Entity representing a company in the database:
- Mapped to `auth.companies` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt)
- Has relationships with: UserRoles, Subscriptions, Addresses, Orders, Configurations

## Security

- All company endpoints require authentication
- Company update requires "Admin" role for the specific company
- Users can only access companies they have a role in
- CNPJ uniqueness is enforced for active companies

## Testing

The Company domain has comprehensive unit tests located in `Fenicia.Auth.Tests/Domains/Company/`.

### Test Coverage

#### CompanyControllerTests
Tests the HTTP endpoint layer including:
- Retrieving companies for authenticated user with pagination
- Updating company information with Admin authorization
- WideEventContext propagation for request tracking
- Controller attribute validation (Authorize, Route, Produces)

**Key test scenarios:**
- Empty company list returns valid empty pagination
- User with companies returns paginated results
- Admin user can update company successfully
- Non-existent company throws ItemNotExistsException
- Non-Admin user throws PermissionDeniedException

#### UpdateCompanyHandlerTests
Tests the business logic layer for company updates including:
- Admin authorization validation
- Company existence and active status checks
- Permission scoping (Admin must have role in specific company)
- Data integrity (IsActive flag preservation)

**Key test scenarios:**
- Admin user successfully updates company name
- Non-existent company throws ItemNotExistsException
- Inactive company throws ItemNotExistsException
- Non-Admin user throws PermissionDeniedException
- User without role in company throws PermissionDeniedException
- Admin in different company cannot update
- Multiple roles including Admin enables update
- Role name matching is case-sensitive ("admin" != "Admin")

#### CheckCompanyExistsHandlerTests
Tests CNPJ uniqueness validation logic including:
- Exact CNPJ matching
- Active/inactive filtering
- Edge cases with special characters

**Key test scenarios:**
- Matching CNPJ returns true
- Non-existent CNPJ returns false
- OnlyActive=true filters inactive companies
- OnlyActive=false includes inactive companies
- Multiple companies with same CNPJ handled correctly

#### GetCompaniesByUserHandlerTests
Tests company retrieval logic including:
- User-company association filtering
- Active company filtering
- Pagination (page boundaries, empty pages)
- Alphabetical sorting by company name
- Multiple roles in same company handling

**Key test scenarios:**
- User with no companies returns empty pagination
- Active company returned correctly
- Inactive companies filtered out
- Multiple companies sorted alphabetically
- Pagination returns correct page of results
- Last page returns remaining items correctly
- Page beyond available returns empty
- Multiple roles in same company returns duplicate entries
- Results scoped to specific user only
- Zero perPage throws InvalidRequestException
