# ForgotPassword Domain

The ForgotPassword domain manages the password recovery process within the Fenicia authentication system. It allows users to reset their passwords when they have forgotten them by generating time-limited verification codes.

## Overview

This domain provides functionality for:
- Initiating the forgot password process (generating a verification code)
- Completing the password reset using the verification code

## Business Logic

### Forgot Password Flow

The domain implements a secure two-step password recovery process:

1. **Initiate Reset (Forgot Password)**
   - User provides their email address
   - System generates a 6-character alphanumeric verification code
   - Code is stored with the user ID, expiration date (default: 1 day), and active status
   - Multiple codes can be generated for the same user

2. **Complete Reset (Reset Password)**
   - User provides email, new password, and verification code
   - System validates the code exists, is active, and has not expired
   - User's password is updated with the new password (hashed)
   - The used code is invalidated (marked as inactive) to prevent reuse

### ForgotPassword Entity
A forgot password record represents a verification code with the following characteristics:
- **Id**: Unique identifier (GUID)
- **UserId**: User who requested the password reset
- **Code**: 6-character alphanumeric verification code
- **ExpirationDate**: When the code expires (default: 1 day from creation)
- **IsActive**: Whether the code is still valid (default: true)

### Security Considerations
- Codes are single-use (invalidated after successful password reset)
- Codes expire after a configurable time period
- Email matching is case-sensitive
- Codes are validated against user ID, active status, and expiration date

## Components

### Controllers

#### ForgotPasswordController
HTTP endpoint controller providing REST API for password recovery operations.
- `POST /ForgotPassword` - Initiates the forgot password process
- `POST /ForgotPassword/reset` - Completes the password reset

### Handlers

#### AddForgotPasswordHandler
Handles the forgot password initiation. Creates a verification code for the user.

#### ResetPasswordHandler
Handles the password reset completion. Validates the code and updates the user's password.

### Commands

#### AddForgotPasswordCommand
Command record for initiating forgot password.
- `Email`: User's email address

#### ResetPasswordCommand
Command record for completing password reset.
- `Email`: User's email address
- `Password`: New password to set
- `Code`: Verification code

## Data Model

### ForgotPasswordModel
Entity representing a forgot password verification code in the database:
- Mapped to `auth.forgotten_passwords` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt)
- Has relationships with: User
- Default expiration: 1 day from creation

## Security

- Endpoints are publicly accessible (AllowAnonymous) for password recovery
- Codes are single-use only
- Codes expire after 1 day
- Email matching is case-sensitive
- Password is hashed before storage

## Testing

The ForgotPassword domain has comprehensive unit tests located in `Fenicia.Auth.Tests/Domains/ForgotPassword/`.

### Test Coverage

#### ForgotPasswordControllerTests
Tests the HTTP endpoint layer including:
- Forgot password initiation
- Password reset completion
- WideEventContext propagation
- Controller attribute validation

**Key test scenarios:**
- User exists - forgot password completes successfully
- User does not exist - throws ItemNotExistsException
- Valid code - resets password successfully
- Invalid code - throws InvalidDataException
- WideEventContext is properly set

#### AddForgotPasswordHandlerTests
Tests the forgot password initiation logic including:
- Code generation
- User validation
- Multiple code handling

**Key test scenarios:**
- Email exists - creates forgot password code successfully
- Email does not exist - throws ItemNotExistsException
- Email case sensitivity - different case throws exception
- Multiple users - creates code for correct user
- Multiple calls - creates multiple codes
- Empty database - throws ItemNotExistsException
- Codes are unique

#### ResetPasswordHandlerTests
Tests the password reset logic including:
- Code validation (existence, active status, expiration)
- Password update
- Code invalidation

**Key test scenarios:**
- Valid code - resets password successfully
- Email does not exist - throws ItemNotExistsException
- Invalid code - throws InvalidDataException
- Inactive code - throws InvalidDataException
- Expired code - throws InvalidDataException
- Code belongs to different user - throws InvalidDataException
- Code used second time - throws InvalidDataException
- Password actually changed after reset
- Empty database - throws ItemNotExistsException
