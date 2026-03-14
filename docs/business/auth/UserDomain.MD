# User Domain

## Overview

The User domain manages user accounts within the platform. It provides CRUD operations, authentication support, and user role management.

Related documentation:
- See [RegisterDomain](./RegisterDomain.MD) for user registration
- See [SecurityDomain](./SecurityDomain.MD) for password hashing
- See [TokenDomain](./TokenDomain.MD) for authentication
- See [UserRoleDomain](./UserRoleDomain.MD) for role management

## Architecture

### Controllers

**UserController**
- Location: `Fenicia.Auth/Domains/User/UserController.cs`

Endpoints:
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/user/module` | Get user's accessible modules |
| GET | `/user/company` | Get user's companies |
| GET | `/user` | List all users (paginated) |
| GET | `/user/{id}` | Get user by ID |
| POST | `/user` | Create new user |
| PATCH | `/user/{id}` | Update user (Admin/God) |
| DELETE | `/user/{id}` | Delete user |
| PATCH | `/user/{id}/password` | Change user password |

### Handlers

| Handler | Description |
|---------|-------------|
| `CreateNewUserHandler` | Creates user with company (registration) |
| `CreateUserHandler` | Creates user for existing company |
| `GetUserHandler` | Lists users with pagination |
| `GetUserByIdHandler` | Gets user by ID |
| `GetByEmailHandler` | Gets user by email |
| `GetUserForRefreshHandler` | Gets user for token refresh |
| `UpdateUserHandler` | Updates user information |
| `UpdateUserPasswordHandler` | Updates user password |
| `UpdatePasswordHandler` | Updates password with verification |
| `DeleteUserHandler` | Deletes user |
| `CheckUserExistsHandler` | Checks if user exists |

### Commands

- `CreateNewUserCommand` - Registration with new company
- `CreateUserCommand` - Create user in existing company
- `UpdateUserCommand` - Update user details
- `UpdateUserPasswordCommand` - Update password
- `DeleteUserCommand` - Delete user

### Responses

- `CreateNewUserResponse` - Created user with company
- `CreateUserResponse` - Created user info
- `GetUserByIdResponse` - User details
- `GetByEmailResponse` - User by email
- `UpdateUserResponse` - Updated user info
- `UpdatePasswordResponse` - Password update result
- `UserListItemResponse` - User list item

## Business Logic

### User Creation

1. Validate email uniqueness
2. Hash password with BCrypt
3. Create user record
4. For registration: create company and Admin role

### User Update

1. Validate user exists
2. For Admin role: verify caller has permission
3. Update user fields

### Password Change

1. Verify current password
2. Hash new password
3. Update user record

## Unit Tests

### UserControllerTests
- Endpoint attribute tests

### CreateNewUserHandlerTests
- Registration flow tests

### CreateUserHandlerTests
- User creation tests

### GetUserHandlerTests
- User retrieval tests

### UpdateUserHandlerTests
- User update tests

### DeleteUserHandlerTests
- User deletion tests

### GetByEmailHandlerTests
- Email lookup tests

### CheckUserExistsHandlerTests
- Existence check tests

### UpdatePasswordHandlerTests
- Password update tests

### UpdateUserPasswordHandlerTests
- Password change tests

### GetUserForRefreshHandlerTests
- Token refresh user lookup tests
