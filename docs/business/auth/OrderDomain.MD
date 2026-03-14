# Order Domain (Auth Module)

## Overview

The Auth module's Order domain handles the creation of module subscription orders. When a company purchases access to modules through the multi-tenant SaaS platform, an order is created along with a subscription and subscription credits.

This is part of the core platform functionality that enables businesses to subscribe to modules.

Related documentation:
- See [ModuleDomain](./ModuleDomain.MD) for module information
- See [CompanyDomain](./CompanyDomain.MD) for company information
- See [UserRoleDomain](./UserRoleDomain.MD) for user-company associations

## Architecture

### Controllers

**OrderController**
- Location: `Fenicia.Auth/Domains/Order/OrderController.cs`
- Endpoint: `POST /order` - Creates a new module subscription order

### Handlers

**CreateNewOrderHandler**
- Location: `Fenicia.Auth/Domains/Order/CreateNewOrder/Handlers/CreateNewOrderHandler.cs`
- Creates order with Approved status
- Automatically creates 1-month subscription
- Automatically adds Basic module if not included
- Creates subscription credits for each module

### Commands

**CreateNewOrderCommand**
- Location: `Fenicia.Auth/Domains/Order/CreateNewOrder/Commands/CreateNewOrderCommand.cs`
- Properties: `UserId`, `CompanyId`, `Modules` (list of module IDs)

### Responses

**CreateNewOrderResponse**
- Location: `Fenicia.Auth/Domains/Order/CreateNewOrder/Responses/CreateNewOrderResponse.cs`
- Properties: `OrderId`

## Business Logic

1. **User Validation**: Verifies user has a role in the company
2. **Module Validation**: Ensures requested modules exist
3. **Auto-Basic Inclusion**: If Basic module not requested, automatically adds it
4. **Order Creation**: Creates order with Approved status
5. **Subscription Creation**: Creates 1-month active subscription
6. **Credit Creation**: Creates subscription credits for each module

## Integration with Other Domains

- **Module**: Validates and retrieves module information
- **UserRole**: Validates user belongs to company
- **Subscription**: Creates subscriptions and credits

## Unit Tests

### CreateNewOrderHandlerTests
- Location: `Fenicia.Auth.Tests/Domains/Order/CreateNewOrderHandlerTests.cs`

**Test Methods:**
- `Handle_WhenValidRequest_CreatesOrderSuccessfully`
- `Handle_WhenUserDoesNotExistInCompany_ThrowsPermissionDeniedException`
- `Handle_WhenModulesNotFound_ThrowsItemNotExistsException`
- `Handle_WhenNoModulesRequested_ReturnsNull`
- `Handle_WhenModuleIsBasicType_DoesNotAddAnotherBasic`
- `Handle_WhenModuleIsNotBasic_AddsBasicModuleAutomatically`
- `Handle_WhenBasicModuleNotFound_ReturnsNull`
- `Handle_WhenDuplicateModuleIds_RemovesDuplicates`

### OrderControllerTests
- Location: `Fenicia.Auth.Tests/Domains/Order/OrderControllerTests.cs`

**Test Methods:**
- `CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ThrowsPermissionDeniedException`
- `CreateNewOrderAsync_WhenModulesDoNotExist_ThrowsItemNotExistsException`
- `CreateNewOrderAsync_WhenValidRequest_ReturnsOkWithOrder`
- `CreateNewOrderAsync_SetsWideEventContextUserId`
- `OrderController_HasAuthorizeAttribute`
- `OrderController_HasRouteAttribute`
- `OrderController_HasProducesAttribute`
