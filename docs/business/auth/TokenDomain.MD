# Token Domain

## Overview

The Token domain handles authentication token generation and refresh. It provides JWT-based authentication with refresh token support.

Related documentation:
- See [SecurityDomain](./SecurityDomain.MD) for password verification
- See [LoginAttemptDomain](./LoginAttemptDomain.MD) for brute-force protection
- See [RefreshTokenDomain](./RefreshTokenDomain.MD) for refresh token management
- See [UserDomain](./UserDomain.MD) for user lookup

## Architecture

### Token Flow

1. **Login** (`POST /token`): Authenticate with email/password
2. **Refresh** (`POST /token/refresh`): Get new tokens using refresh token

### Controllers

**TokenController**
- Location: `Fenicia.Auth/Domains/Token/TokenController.cs`

Endpoints:
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/token` | Authenticate and get tokens |
| POST | `/token/refresh` | Refresh tokens with refresh token |

### Handlers

| Handler | Description |
|---------|-------------|
| `GenerateTokenHandler` | Validates credentials, checks login attempts, returns user info |
| `GenerateTokenStringHandler` | Generates JWT token with user claims |
| `GenerateRefreshTokenHandler` | Generates refresh token (see RefreshTokenDomain) |
| `ValidateTokenHandler` | Validates refresh token (see RefreshTokenDomain) |
| `InvalidateRefreshTokenHandler` | Invalidates refresh token (see RefreshTokenDomain) |
| `GetUserForRefreshHandler` | Gets user info for token refresh |

### Queries

**GenerateTokenQuery**
- `Email` (string) - User's email
- `Password` (string) - User's password

### Responses

**TokenResponse**
- `AccessToken` (string) - JWT token
- `RefreshToken` (string) - Refresh token
- `User` (UserResponse) - User information

**GenerateTokenResponse**
- `Id` (Guid) - User ID
- `Name` (string) - User name
- `Email` (string) - User email

**UserResponse**
- `Id` (Guid) - User ID
- `Name` (string) - User name
- `Email` (string) - User email

## Authentication Flow

### POST /token (Login)

1. Validate login attempt count (block after 5 attempts)
2. Find user by email
3. If not found, increment attempts and delay
4. Verify password
5. If valid, clear login attempts and return tokens
6. If invalid, increment attempts and delay
7. Apply progressive delay (up to 5 seconds)

### POST /token/refresh

1. Validate refresh token
2. If invalid, return error
3. Invalidate old refresh token
4. Get user information
5. Generate new JWT and refresh token
6. Return new tokens

## Security Features

1. **Brute Force Protection**: Blocks after 5 failed attempts
2. **Progressive Delay**: Increases delay with failed attempts
3. **JWT Expiration**: Tokens expire in 3 hours
4. **Refresh Token Rotation**: New refresh token on each use

## Unit Tests

### GenerateTokenHandlerTests

Tests verify authentication flow, password validation, and login attempt tracking.

### GenerateTokenStringHandlerTests

Tests verify JWT generation with correct claims.

### TokenControllerTests

Tests verify token endpoint behavior and error handling.
