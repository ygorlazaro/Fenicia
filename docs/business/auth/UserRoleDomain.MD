# UserRole Domain

## Overview

The UserRole domain manages user-company role associations. It handles which users belong to which companies and what roles they have.

Related documentation:
- See [CompanyDomain](./CompanyDomain.MD) for company management
- See [UserDomain](./UserDomain.MD) for user management

## Architecture

### Extensions

**UserRoleExtensions**
- Location: `Fenicia.Auth/Domains/UserRole/UserRoleExtensions.cs`

Methods:
| Method | Description |
|--------|-------------|
| `AnyIdAndCompanyAsync` | Check if user belongs to company |
| `HasRoleAsync` | Check if user has specific role in company |

### Handlers

| Handler | Description |
|---------|-------------|
| `GetCompaniesByUserHandler` | Get all companies for a user |
| `GetUserCompaniesHandler` | Get companies with role info |

### Queries

**GetCompaniesByUserQuery**
- `UserId` (Guid) - User identifier
- `Page` (int) - Page number
- `PerPage` (int) - Items per page

### Responses

**GetUserCompaniesResponse**
- `Companies` (List<CompanyResponse>) - User's companies

**CompanyResponse**
- `Id` (Guid) - Company ID
- `Name` (string) - Company name
- `Role` (string) - User's role in company

## Business Logic

### User-Company Association

Users can belong to multiple companies with different roles in each. The `AuthUserRoles` table maintains:
- User ID
- Company ID
- Role ID

### Role Checking

The system provides extension methods to:
1. Check if a user belongs to a company
2. Check if a user has a specific role in a company

## Usage Flow

1. User authenticates
2. System retrieves user's companies
3. System checks user's roles in each company
4. Access control is applied based on role

## Integration

This domain is used by:
- Authentication to verify user-company relationship
- Authorization to check role permissions
- Order creation to validate company association
- Any operation requiring company context
