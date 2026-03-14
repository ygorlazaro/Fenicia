# RefreshToken Domain

## Overview

The RefreshToken domain manages refresh tokens for session management and token renewal. It uses Redis for secure, distributed token storage with automatic expiration.

Related documentation:
- See [LoginAttemptDomain](./LoginAttemptDomain.MD) for login protection
- See [ForgotPasswordDomain](./ForgotPasswordDomain.MD) for password recovery

## Architecture

### Storage

Refresh tokens are stored in Redis with:
- Key format: `refresh_token:{token}`
- Value: JSON-serialized token data
- TTL: 7 days (automatically expires)

### Handlers

| Handler | Description |
|---------|-------------|
| `GenerateRefreshTokenHandler` | Generates cryptographically secure tokens |
| `ValidateTokenHandler` | Validates token existence, ownership, and expiration |
| `InvalidateRefreshTokenHandler` | Soft-deletes tokens (marks as inactive) |

### Model

**RefreshTokenModel**
- `Token` (string) - Base64-encoded random token
- `ExpirationDate` (DateTime) - Token expiration timestamp
- `UserId` (Guid) - Associated user ID
- `IsActive` (bool) - Token active status

### Queries

**ValidateTokenQuery**
- `UserId` (Guid) - User to validate token for
- `RefreshToken` (string) - The refresh token string

### Responses

**ValidateTokenResponse**
- `Token` (string) - Token string
- `ExpirationDate` (DateTime) - Expiration timestamp
- `UserId` (Guid) - User ID
- `IsActive` (bool) - Active status

**InvalidateRefreshTokenResponse**
- `Token` (string) - Token string
- `ExpirationDate` (DateTime) - Expiration timestamp
- `UserId` (Guid) - User ID
- `IsActive` (bool, settable) - Active status (set to false when invalidated)

## Token Lifecycle

### 1. Generation
- Client authenticates with credentials
- Server generates 32-byte cryptographically secure random token
- Token is Base64-encoded and stored in Redis with 7-day TTL
- Token is returned to client

### 2. Validation
- Client sends refresh token with user ID
- Server checks:
  - Token exists in Redis
  - Token is marked as active
  - Token has not expired
  - Token belongs to the specified user
- Returns true if all checks pass, false otherwise

### 3. Invalidation (Logout)
- Client requests logout
- Server sets token's `IsActive` to false
- Token remains in Redis until TTL expires
- Any subsequent validation will fail

## Security Features

1. **Cryptographically Secure**: Uses `RandomNumberGenerator` for token generation
2. **Short Expiration**: 7-day validity limits exposure window
3. **User Binding**: Tokens are tied to specific user IDs
4. **Soft Delete**: Inactive tokens are marked rather than deleted (audit trail)
5. **Graceful Failures**: Validation returns false for errors rather than throwing exceptions

## Unit Tests

### GenerateRefreshTokenHandlerTests

**Test Methods:**
- `Handle_GeneratesValidRefreshToken` - Valid Base64 string of correct length
- `Handle_GeneratesUniqueTokensForEachCall` - Each call produces unique token
- `Handle_SavesTokenToRedisWithCorrectKey` - Correct key format in Redis
- `Handle_SavesTokenToRedisWithCorrectValue` - Correct value content in Redis
- `Handle_SetsCorrectExpirationTime` - 7-day TTL is set
- `Handle_TokenIsActiveByDefault` - Token created as active
- `Handle_ForDifferentUsers_GeneratesDifferentTokens` - User-specific tokens
- `Handle_VerifiesRedisSetValueCanBeDeserialized` - JSON serialization works
- `Handle_MultipleTokensForSameUser_AreUnique` - Multiple tokens are unique

### ValidateTokenHandlerTests

**Test Methods:**
- `Handle_WhenTokenIsValidAndActive_ReturnsTrue` - Valid token passes
- `Handle_WhenTokenDoesNotExistInRedis_ReturnsFalse` - Missing token fails
- `Handle_WhenTokenIsInactive_ReturnsFalse` - Inactive token fails
- `Handle_WhenTokenIsExpired_ReturnsFalse` - Expired token fails
- `Handle_WhenTokenBelongsToDifferentUser_ReturnsFalse` - Wrong user fails
- `Handle_WhenRefreshTokenIsNull_ThrowsArgumentException` - Null token throws
- `Handle_WhenTokenIsExpiringSoon_ReturnsTrue` - Near-expiry still valid
- `Handle_WhenTokenHasExactlyCurrentExpirationTime_ReturnsFalse` - Exact expiry fails
- `Handle_WhenMalformedJsonInRedis_ReturnsFalse` - Corrupted data handled
- `Handle_WhenRefreshTokenIsWhitespace_ThrowsArgumentException` - Whitespace throws
- `Handle_WhenRedisThrowsException_ReturnsFalse` - Redis errors handled
- `Handle_WhenTokenIsNullAfterDeserialization_ReturnsFalse` - Null after parse handled

### InvalidateRefreshTokenHandlerTests

**Test Methods:**
- `Handler_WhenTokenExists_SetsIsActiveToFalse` - Successfully invalidates
- `Handler_WhenTokenDoesNotExist_ReturnsSilently` - Missing token handled
- `Handler_WhenTokenIsNull_ThrowsArgumentNullException` - Null throws
- `Handler_WhenTokenIsEmptyString_ReturnsSilently` - Empty string handled
- `Handler_WhenTokenIsAlreadyInactive_StillUpdates` - Idempotent operation
- `Handler_WhenTokenIsExpired_StillInvalidates` - Expired can be invalidated
- `Handler_PreservesOtherTokenProperties` - Other fields unchanged
- `Handler_WhenMalformedJsonInRedis_ReturnsSilently` - Corrupted data handled
- `Handler_MultipleInvalidationsForSameToken_WorksCorrectly` - Idempotent
- `Handler_VerifiesCorrectTTLIsSet` - TTL preserved on update
- `Handler_WhenRedisThrowsException_SilentlyIgnores` - Redis errors handled
- `Handler_WhenJsonDeserializationFails_ReturnsSilently` - Parse errors handled
