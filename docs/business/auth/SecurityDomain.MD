# Security Domain

## Overview

The Security domain provides core security functionality for password hashing and verification. It uses BCrypt for secure password storage.

Related documentation:
- See [RegisterDomain](./RegisterDomain.MD) for user registration
- See [TokenDomain](./TokenDomain.MD) for authentication

## Components

### HashStringExtensions

**Location:** `Fenicia.Auth/Domains/Security/HashStringExtensions.cs`

Extension method for hashing strings using BCrypt.

**Method:**
```csharp
public static string Hash(this string original)
```

- Uses BCrypt with work factor 12
- Throws `InvalidRequestException` if input is null or empty

### VerifyPasswordService

**Location:** `Fenicia.Auth/Domains/Security/Services/VerifyPasswordService.cs`

Service for verifying passwords against BCrypt hashes.

**Method:**
```csharp
public virtual bool Handle(string password, string hashedPassword)
```

- Returns true if password matches
- Returns false for null/empty inputs or any errors
- Does not throw exceptions (graceful failure)

## Security Features

1. **BCrypt Hashing**: Industry-standard password hashing
2. **Work Factor 12**: Balanced security and performance
3. **Graceful Verification**: No information leakage through errors

## Unit Tests

### VerifyPasswordServiceTests

Tests verify password matching, null handling, and error conditions.
