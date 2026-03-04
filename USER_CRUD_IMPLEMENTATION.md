# User CRUD Implementation Guide

## Overview
This document describes the complete User CRUD implementation with role-based access control, company management, and password reset functionality.

## Backend Implementation

### Controller: `UserController.cs`
**Location:** `/home/ygor/Projects/Fenicia/Fenicia.ERP.API/Areas/Auth/UserController.cs`

#### Endpoints:

1. **GET /user** - List users with pagination
   - Query Parameters: `page`, `pageSize`, `searchTerm`
   - Authorization: God role only
   - Returns: Paginated list of users ordered alphabetically by name

2. **GET /user/{userId}** - Get user by ID
   - Authorization: God (all users), Admin (users from their companies only)
   - Returns: User details with companies and roles

3. **POST /user** - Create new user
   - Authorization: God and Admin
   - Admin restriction: Can only create users for companies they have access to
   - Body: `CreateUserQuery` (email, password, name, companiesRoles)

4. **PATCH /user/{userId}** - Update user
   - Authorization: God and Admin
   - Admin restriction: Can only update users from their companies
   - Body: `UpdateUserQuery` (name?, email?, companiesRoles?)

5. **DELETE /user/{userId}** - Soft delete user
   - Authorization: God only
   - Prevents self-deletion
   - Returns: Success/failure message

6. **PATCH /user/{userId}/password** - Change user password
   - Authorization: God and Admin
   - Admin restriction: Can only change passwords for users from their companies
   - Body: `ChangeUserPasswordQuery` (newPassword)
   - Password is hashed using BCrypt

### Authorization Rules

#### God Users:
- Can perform all operations on all users
- Can delete users
- No company restrictions

#### Admin Users:
- Can create, update, and change passwords
- Can only manage users from companies they have access to
- Cannot delete users
- Cannot manage God users

### Data Models

#### DTOs Created:

1. **ListUsersQuery/ListUsersResponse** - Pagination and search
2. **CreateUserQuery/CreateUserResponse** - User creation with companies/roles
3. **UpdateUserQuery/UpdateUserResponse** - User updates
4. **DeleteUserQuery/DeleteUserResponse** - User deletion
5. **ChangeUserPasswordQuery/ChangeUserPasswordResponse** - Password changes

#### Company-Role Relationship:
```csharp
public record UserCompanyRoleCommand(
    Guid CompanyId,
    Guid RoleId
);
```

Users can have multiple companies, each with a specific role.

### Handlers

All handlers follow the CQRS pattern:
- `ListUsersHandler` - Lists users with pagination and alphabetical sorting
- `CreateUserHandler` - Creates user with optional company roles
- `UpdateUserHandler` - Updates user data and company roles
- `DeleteUserHandler` - Soft deletes users
- `ChangeUserPasswordHandler` - Changes password with BCrypt hashing

## Frontend Implementation

### Routes
**File:** `/home/ygor/Projects/Fenicia/fenicia-web/src/routes.js`

Added route: `/auth/user` → User Management component

### Navigation
**File:** `/home/ygor/Projects/Fenicia/fenicia-web/src/_nav.js`

Added "User Management" menu item with user icon

### Header
**File:** `/home/ygor/Projects/Fenicia/fenicia-web/src/components/AppHeader.js`

Added "Users" link in header navigation

### Components Created:

1. **UserList** (`/views/auth/user/index.js`)
   - Main CRUD screen
   - Features:
     - Search by name/email
     - Pagination (10 users per page)
     - Alphabetical sorting
     - Create, Edit, Delete users
     - Change password
     - Display user companies and roles

2. **UserModal** (`/components/UserModal.js`)
   - Create/Edit user form
   - Fields: Name, Email, Password (create only)
   - Multi-select for companies with role assignment
   - Validation

3. **UserPasswordModal** (`/components/UserPasswordModal.js`)
   - Password change form
   - New password + confirmation
   - Minimum 6 characters
   - BCrypt hashing on backend

### Features:

#### Pagination
- Configurable page size (default: 10)
- Previous/Next navigation
- Shows total count and pages

#### Search
- Search by name or email
- Real-time filtering
- Reset functionality

#### Company Management
- Select multiple companies for user
- Assign specific role per company
- Visual badges showing company-role pairs

#### Password Management
- Admin can change any user's password (with restrictions)
- Password confirmation
- BCrypt hashing

## Unit Tests

### Test Files Created:

1. **CreateUserHandlerTests.cs**
   - Valid user creation
   - Email exists validation
   - Company/role assignment
   - Password hashing verification

2. **UpdateUserHandlerTests.cs**
   - Name/email updates
   - Company role updates
   - Email uniqueness validation
   - User not found handling

3. **DeleteUserHandlerTests.cs**
   - Soft delete verification
   - User not found handling
   - Already deleted handling

4. **ListUsersHandlerTests.cs**
   - Pagination
   - Alphabetical ordering
   - Search filtering
   - Company/role inclusion

5. **ChangeUserPasswordHandlerTests.cs**
   - Password change
   - BCrypt hashing verification
   - User not found handling
   - Timestamp updates

## Testing

### Backend Tests
```bash
cd Fenicia.Auth.Tests
dotnet test
```

### Frontend Testing
```bash
cd fenicia-web
npm run build
npm run serve
# Access http://localhost:4173/auth/user
```

### Manual Testing Checklist:

#### God User:
- [ ] List all users
- [ ] Create user with multiple companies
- [ ] Edit user from any company
- [ ] Delete user
- [ ] Change any user's password

#### Admin User:
- [ ] List users from accessible companies
- [ ] Create user for accessible companies
- [ ] Edit user from accessible companies
- [ ] Cannot delete users
- [ ] Change password for accessible companies
- [ ] Cannot access users from other companies

#### Features:
- [ ] Pagination works correctly
- [ ] Search filters by name and email
- [ ] Users ordered alphabetically
- [ ] Company-role assignment works
- [ ] Password is hashed (check database)
- [ ] Soft delete (Deleted timestamp set)

## API Examples

### List Users
```bash
GET /user?page=1&pageSize=10&searchTerm=john
Authorization: Bearer {token}
x-company: {companyId}
```

### Create User
```bash
POST /user
Authorization: Bearer {token}
x-company: {companyId}
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "securepassword123",
  "name": "John Doe",
  "companiesRoles": [
    { "companyId": "guid-1", "roleId": "guid-2" }
  ]
}
```

### Update User
```bash
PATCH /user/{userId}
Authorization: Bearer {token}
x-company: {companyId}
Content-Type: application/json

{
  "name": "John Doe Updated",
  "companiesRoles": [
    { "companyId": "guid-1", "roleId": "guid-3" }
  ]
}
```

### Change Password
```bash
PATCH /user/{userId}/password
Authorization: Bearer {token}
x-company: {companyId}
Content-Type: application/json

{
  "newPassword": "newpassword123"
}
```

### Delete User
```bash
DELETE /user/{userId}
Authorization: Bearer {token}
x-company: {companyId}
```

## Database Schema

### Tables Involved:
- `auth.users` - User accounts
- `auth.users_roles` - User company-role assignments
- `auth.companies` - Companies
- `auth.roles` - Roles

### Soft Delete:
- `users.deleted` column stores deletion timestamp
- NULL = active, NOT NULL = deleted

## Security Considerations

1. **Password Hashing**: BCrypt with salt (12 rounds)
2. **Authorization**: Role-based (God/Admin)
3. **Company Isolation**: Admin users restricted to their companies
4. **Self-Delete Prevention**: Users cannot delete themselves
5. **Email Uniqueness**: Enforced at database and application level

## Future Enhancements

- [ ] Bulk user import/export
- [ ] User activity logging
- [ ] Password strength requirements
- [ ] Two-factor authentication
- [ ] User profile pictures
- [ ] Email notifications on password change
- [ ] Account lockout after failed attempts
- [ ] Password history tracking

## Troubleshooting

### Common Issues:

1. **"User not found" for Admin**
   - Admin can only see users from their companies
   - Check AuthUserRole table for company access

2. **Cannot create user**
   - Verify email doesn't exist
   - Check company/role IDs are valid
   - Ensure admin has access to specified companies

3. **Password not changing**
   - Verify minimum 6 characters
   - Check passwords match in modal
   - Ensure user exists and is not deleted

4. **Pagination not working**
   - Check page and pageSize parameters
   - Verify total_count in response
   - Ensure searchTerm is encoded properly
