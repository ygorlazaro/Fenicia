# StockMovement Domain

The StockMovement domain tracks inventory movements within a company. It provides CRUD operations and comprehensive stock analytics.

## Overview

This domain provides functionality for:
- Retrieving stock movements with date filtering
- Creating new stock movements
- Updating existing stock movements (Admin only)
- Retrieving stock movement dashboard analytics

## Business Logic

### StockMovement Entity
A stock movement represents an inventory transaction with the following characteristics:
- **Id**: Unique identifier (GUID)
- **Quantity**: Amount of product moved
- **Date**: Date of the movement
- **Price**: Price per unit
- **Type**: Movement type (In/Out)
- **ProductId**: Associated product
- **CustomerId**: Customer for sales (optional)
- **SupplierId**: Supplier for purchases (optional)
- **EmployeeId**: Employee who processed the movement (optional)
- **OrderId**: Associated order (optional)
- **Reason**: Reason for the movement

### Movement Types
- **In**: Stock additions (purchases, returns, adjustments)
- **Out**: Stock reductions (sales, damage, losses)

### Product Quantity Updates
When a stock movement is created:
- For "In" movements: Product quantity increases
- For "Out" movements: Product quantity decreases

## Components

### Controllers

#### StockMovementController
HTTP endpoint controller providing REST API for stock movement operations.

- `GET /StockMovement` - Retrieves stock movements filtered by date range
- `POST /StockMovement` - Creates a new stock movement
- `PATCH /StockMovement/{id}` - Updates a stock movement (Admin only)
- `GET /StockMovement/dashboard` - Retrieves analytics dashboard

### Handlers

#### GetStockMovementHandler
Handles retrieval of stock movements with date range filtering. Returns movements with related entity information.

#### AddStockMovementHandler
Handles stock movement creation requests. Creates movement and updates product quantity.

#### UpdateStockMovementHandler
Handles stock movement update requests.

#### GetStockMovementDashboardHandler
Handles stock movement analytics requests. Returns:
- Movement history
- Monthly in/out totals
- Top moved products
- Stock turnover rates

### Commands

#### AddStockMovementCommand
Command record for creating a new stock movement.
- `Id`: Unique identifier
- `Quantity`: Quantity moved
- `Date`: Movement date
- `Price`: Price per unit
- `Type`: Movement type (In/Out)
- `ProductId`: Product ID
- `CustomerId`: Customer ID (optional)
- `SupplierId`: Supplier ID (optional)
- `EmployeeId`: Employee ID (optional)
- `OrderId`: Order ID (optional)
- `Reason`: Reason for movement

#### UpdateStockMovementCommand
Command record for updating a stock movement.
- `Id`: Movement ID to update
- `Quantity`: Updated quantity
- `Date`: Updated date
- `Price`: Updated price
- `Type`: Updated type
- `ProductId`: Updated product ID
- `CustomerId`: Updated customer ID
- `SupplierId`: Updated supplier ID
- `EmployeeId`: Updated employee ID
- `OrderId`: Updated order ID
- `Reason`: Updated reason

### Queries

#### GetStockMovementQuery
Query for retrieving stock movements.
- `StartDate`: Start of date range
- `EndDate`: End of date range
- `Page`: Page number
- `PerPage`: Items per page

#### GetStockMovementDashboardQuery
Query for retrieving dashboard analytics.
- `Days`: Number of days to analyze
- `TopLimit`: Number of top products

### Responses

#### GetStockMovementResponse
Response model for a stock movement.
- `Id`: Movement ID
- `ProductId`: Product ID
- `ProductName`: Product name
- `Quantity`: Quantity moved
- `Date`: Movement date
- `Price`: Price per unit
- `Type`: Movement type
- `CustomerId`: Customer ID
- `CustomerName`: Customer name
- `SupplierId`: Supplier ID
- `SupplierName`: Supplier name
- `EmployeeId`: Employee ID
- `EmployeeName`: Employee name
- `OrderId`: Order ID
- `Reason`: Reason

#### AddStockMovementResponse
Response for created movement.
- `Id`: Movement ID
- `ProductId`: Product ID
- `Quantity`: Quantity
- `Date`: Date
- `Price`: Price
- `Type`: Type
- `CustomerId`: Customer ID
- `SupplierId`: Supplier ID
- `EmployeeId`: Employee ID
- `OrderId`: Order ID
- `Reason`: Reason

#### UpdateStockMovementResponse
Response for updated movement.
- `Id`: Movement ID
- `ProductId`: Product ID
- `Quantity`: Quantity
- `Date`: Date
- `Price`: Price
- `Type`: Type
- `CustomerId`: Customer ID
- `SupplierId`: Supplier ID
- `EmployeeId`: Employee ID
- `OrderId`: Order ID
- `Reason`: Reason

#### StockMovementDashboardResponse
Response containing dashboard analytics.
- `History`: Recent movements
- `MonthlyInOut`: Monthly totals
- `TopMovedProducts`: Top products
- `TurnoverRates`: Turnover rates

## Data Model

### StockMovementModel
Entity representing a stock movement in the database:
- Mapped to `basic.stock_movements` table
- Inherits from `BaseModel`
- Has relationships with: Product, Customer, Supplier, Employee, Order

## Security

- All stock movement endpoints require authentication
- Stock movement updates require "Admin" role

## Testing

The StockMovement domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/StockMovement/`.

### Test Coverage

#### StockMovementControllerTests
Tests the HTTP endpoint layer including:
- Retrieving stock movements with date filtering
- Creating new stock movements
- Updating stock movements (Admin authorization)
- Dashboard analytics retrieval
- Controller attribute validation

#### AddStockMovementHandlerTests
Tests stock movement creation including:
- Movement entity creation
- Product quantity updates
- Database persistence

#### GetStockMovementHandlerTests
Tests stock movement retrieval with date filtering.

#### UpdateStockMovementHandlerTests
Tests stock movement update logic.

#### GetStockMovementDashboardHandlerTests
Tests dashboard analytics including:
- Monthly in/out calculations
- Top products identification
- Turnover rate calculations
