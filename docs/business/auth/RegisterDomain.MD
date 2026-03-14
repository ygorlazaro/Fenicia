# Register Domain

## Overview

The Register domain handles new user registration along with company creation. This is the entry point for new businesses to join the platform.

When a new user registers:
1. A user account is created
2. A new company (tenant) is created
3. The user is assigned Admin role for their company

This makes the registering user the first Admin of their company, giving them full control to:
- Invite other users
- Assign roles
- Configure company settings
- Subscribe to modules

Related documentation:
- See [CompanyDomain](./CompanyDomain.MD) for company management
- See [UserRoleDomain](./UserRoleDomain.MD) for role management

## Architecture

### Controllers

**RegisterController**
- Location: `Fenicia.Auth/Domains/Register/RegisterController.cs`
- Endpoint: `POST /register` - Register new user with company

This is a public endpoint (marked with `[AllowAnonymous]`) to allow self-service registration.

### Handlers

**CreateNewUserHandler**
- Location: `Fenicia.Auth/Domains/User/Handlers/CreateNewUserHandler.cs`
- Creates user with hashed password
- Creates new company
- Assigns Admin role to user for the company

### Commands

The handler uses `CreateNewUserCommand` which contains:
- `Email` - User's email (used as username)
- `Password` - Plain text password (hashed before storage)
- `Name` - User's display name
- `Company` - Company details containing:
  - `Name` - Company name
  - `Cnpj` - Company CNPJ (Brazilian business registration)

### Responses

**CreateNewUserResponse**
- Returns created user ID, name, email, and company information

## Registration Flow

1. **Validation**
   - Check email doesn't already exist
   - Check CNPJ doesn't already exist
   - Verify Admin role exists in system

2. **User Creation**
   - Hash password using secure hashing
   - Create user record

3. **Company Creation**
   - Create new company with provided name and CNPJ

4. **Role Assignment**
   - Create user role entry with Admin role
   - Links user to company with Admin permissions

5. **Response**
   - Return user and company information to client

## Business Rules

### Email Uniqueness
- Each user must have a unique email address
- System-wide (not per-company)

### CNPJ Uniqueness
- Each company must have a unique CNPJ
- CNPJ is the primary identifier for companies

### Required Role
- Admin role must exist in the system for registration to succeed
- This is a system-level requirement

### Password Security
- Passwords are hashed before storage
- Uses secure hashing algorithm (BCrypt)

## Unit Tests

### RegisterControllerTests

**Test Methods:**
- `CreateNewUserAsync_WhenEmailAlreadyExists_ThrowsArgumentException` - Duplicate email validation
- `CreateNewUserAsync_WhenCompanyAlreadyExists_ThrowsArgumentException` - Duplicate CNPJ validation
- `CreateNewUserAsync_WhenAdminRoleDoesNotExist_ThrowsArgumentException` - Admin role validation
- `CreateNewUserAsync_WhenValidRequest_ReturnsOkWithUser` - Successful registration
- `CreateNewUserAsync_SetsWideEventContextUserId` - Tracking context setup
- `RegisterController_HasAllowAnonymousAttribute` - Public endpoint verification
- `RegisterController_HasRouteAttribute` - Route configuration
- `RegisterController_HasApiControllerAttribute` - API controller verification
- `RegisterController_HasProducesAttribute` - JSON response configuration

## Integration with Other Domains

- **User**: Creates user records
- **Company**: Creates company records
- **UserRole**: Creates role assignments
- **Role**: References Admin role
- **Security**: Password hashing

## API Usage

### Request
```json
POST /register
{
  "email": "admin@newcompany.com",
  "password": "securePassword123",
  "name": "John Doe",
  "company": {
    "name": "New Company LTDA",
    "cnpj": "12.345.678/0001-90"
  }
}
```

### Response (201 Created)
```json
{
  "id": "user-guid",
  "name": "John Doe",
  "email": "admin@newcompany.com",
  "company": {
    "id": "company-guid",
    "name": "New Company LTDA",
    "cnpj": "12.345.678/0001-90"
  }
}
```

### Error Responses (400 Bad Request)
- `"Email already exists"` - Duplicate email
- `"Company already exists"` - Duplicate CNPJ
- `"Admin role not found"` - System configuration error
