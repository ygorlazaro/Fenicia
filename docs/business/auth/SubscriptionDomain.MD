# Subscription Domain

## Overview

The Subscription domain manages user profiles within the subscription/company context. It provides endpoints for retrieving user subscription information.

Related documentation:
- See [CompanyDomain](./CompanyDomain.MD) for company management
- See [ModuleDomain](./ModuleDomain.MD) for module subscriptions
- See [UserDomain](./UserDomain.MD) for user management

## Architecture

### Controllers

**SubscriptionController**
- Location: `Fenicia.Auth/Domains/Subscription/SubscriptionController.cs`

Endpoints:
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/subscription/profile` | Get current user's profile |

### Handlers

**GetUserProfileHandler**
- Location: `Fenicia.Auth/Domains/Subscription/Handlers/GetUserProfileHandler.cs`
- Retrieves user profile with subscription information

### Queries

**GetUserProfileQuery**
- `UserId` (Guid) - User identifier

### Responses

**GetUserProfileResponse**
- `User` (UserCompanyResponse) - User and company information
- `Subscriptions` (List<UserSubscriptionResponse>) - Active subscriptions

**UserCompanyResponse**
- `Id` (Guid) - User ID
- `Name` (string) - User name
- `Email` (string) - User email
- `Company` (Company info)

**UserSubscriptionResponse**
- `Id` (Guid) - Subscription ID
- `Status` (SubscriptionStatus) - Subscription status
- `StartDate` (DateTime) - Subscription start
- `EndDate` (DateTime) - Subscription end
- `Modules` (List of module info) - Subscribed modules

## Usage Flow

1. Authenticated user requests profile
2. System retrieves user information
3. System retrieves user's company
4. System retrieves active subscriptions
5. Returns complete profile with subscription details

## Integration

This domain ties together:
- User information
- Company/tenant context
- Module subscriptions
- Subscription validity periods
