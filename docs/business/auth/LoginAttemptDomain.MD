# LoginAttempt Domain

## Overview

The LoginAttempt domain provides brute-force protection for the authentication system by tracking failed login attempts per email address. It uses an in-memory cache to store attempt counters with automatic expiration.

## Architecture

### Services

#### 1. LoginAttemptService
**Location:** `Fenicia.Auth/Domains/LoginAttempt/Services/LoginAttemptService.cs`

Responsible for retrieving login attempt counts from memory cache.

**Key Methods:**
- `Handle(string email)` - Retrieves the current login attempt count for an email. Returns 0 if no attempts exist.

**Cache Key Format:** `login-attempt:{lowercase_email}`

**Behavior:**
- Returns 0 when no attempts exist (not null or -1)
- Normalizes email to lowercase using `CultureInfo.InvariantCulture`
- Throws `ArgumentNullException` for null email

#### 2. IncrementAttemptsService
**Location:** `Fenicia.Auth/Domains/LoginAttempt/Services/IncrementAttemptsService.cs`

Responsible for incrementing login attempt counters in memory cache.

**Key Methods:**
- `SetKey(string email)` - Increments the attempt counter. Initializes to 1 if no counter exists.

**Configuration:**
- Expiration: 15 minutes (configurable via `ExpirationMinutes` constant)
- Cache key prefix: `login-attempt:`

**Behavior:**
- Increments existing counter atomically
- Sets cache entry with absolute expiration of 15 minutes
- Normalizes email to lowercase

#### 3. ResetAttemptsService
**Location:** `Fenicia.Auth/Domains/LoginAttempt/Services/ResetAttemptsService.cs`

Responsible for resetting login attempt counters after successful login.

**Key Methods:**
- `Handle(string email)` - Removes the attempt counter from cache.

**Behavior:**
- Trims and normalizes email to lowercase
- Safely handles cases where no attempts exist (no-op)
- Throws `ArgumentNullException` for null email
- Throws `ArgumentException` for empty/whitespace-only email

## Usage Flow

1. **Failed Login Attempt:**
   - Call `IncrementAttemptsService.SetKey(email)` to increment the counter
   - Call `LoginAttemptService.Handle(email)` to check current attempts
   - If attempts exceed threshold, block the login

2. **Successful Login:**
   - Call `ResetAttemptsService.Handle(email)` to clear the counter
   - This allows the user to start fresh

## Unit Tests

### LoginAttemptServiceTests
**Location:** `Fenicia.Auth.Tests/Domains/LoginAttempt/LoginAttemptServiceTests.cs`

Tests cover:
- Retrieval of existing attempt counts
- Default value (0) when no attempts exist
- Case-insensitive email handling
- Proper exception handling for null input
- Isolation between different email addresses

**Test Methods:**
- `Handle_WhenNoAttemptsExist_ReturnsZero`
- `Handle_WhenAttemptsExist_ReturnsAttemptCount`
- `Handle_WhenEmailHasDifferentCase_ReturnsCorrectCount`
- `Handle_WhenEmailIsNull_ThrowsArgumentNullException`
- `Handle_WhenEmailIsEmpty_ReturnsZero`
- `Handle_WhenEmailContainsSpaces_ReturnsZero`
- `Handle_WithMultipleDifferentEmails_ReturnsCorrectCounts`
- `Handle_WhenAttemptCountIsHigh_ReturnsCorrectCount`

### IncrementAttemptsServiceTests
**Location:** `Fenicia.Auth.Tests/Domains/LoginAttempt/IncrementAttemptsServiceTests.cs`

Tests cover:
- Initial counter creation (sets to 1)
- Proper increment of existing counters
- Case-insensitive email handling
- Proper exception handling for null input
- Isolation between different email addresses
- Handling of special characters in email addresses

**Test Methods:**
- `Handle_WhenNoPreviousAttempts_SetsCountToOne`
- `Handle_WhenPreviousAttemptsExist_IncrementsCount`
- `Handle_WhenEmailHasDifferentCase_NormalizesToLowerCase`
- `Handle_WhenEmailIsNull_ThrowsArgumentNullException`
- `Handle_WhenEmailIsEmpty_SetsCountForEmptyKey`
- `Handle_MultipleIncrementsForSameEmail_IncrementsCorrectly`
- `Handle_MultipleDifferentEmails_TracksSeparately`
- `Handle_WhenExpirationIsSet_ExpiresAfterTimeSpan`
- `Handle_WithHighAttemptCount_IncrementsCorrectly`
- `Handle_WhenEmailContainsSpecialCharacters_HandlesCorrectly`

### ResetAttemptsServiceTests
**Location:** `Fenicia.Auth.Tests/Domains/LoginAttempt/ResetAttemptsServiceTests.cs`

Tests cover:
- Removal of existing attempt counters
- Safe handling when no attempts exist
- Case-insensitive email handling
- Proper exception handling for null/empty input
- Isolation between different email addresses
- Handling of multiple reset operations

**Test Methods:**
- `Handle_WhenAttemptsExist_RemovesAttempts`
- `Handle_WhenNoAttemptsExist_CompletesSuccessfully`
- `Handle_WhenEmailHasDifferentCase_RemovesCorrectAttempts`
- `Handle_WhenEmailIsNull_ThrowsArgumentNullException`
- `Handle_WhenEmailIsEmpty_RemovesEmptyKey`
- `Handle_WhenMultipleEmailsExist_RemovesOnlySpecifiedEmail`
- `Handle_WhenHighAttemptCountExists_RemovesSuccessfully`
- `Handle_MultipleResetsForSameEmail_CompletesSuccessfully`

## Security Considerations

1. **Cache Expiration:** Counter entries expire after 15 minutes of inactivity, allowing legitimate users to retry after a cooldown period.

2. **Case Normalization:** All emails are normalized to lowercase to prevent case-sensitive bypass attempts.

3. **Isolation:** Each email address has its own independent counter, preventing a single account's lockout from affecting others.

4. **No Timing Attack Mitigation:** The current implementation does not include deliberate delays. Consider adding rate limiting at the API level for production environments.
