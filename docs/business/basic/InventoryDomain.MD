# Inventory Domain

The Inventory domain manages inventory tracking and analytics within a company in the Fenicia system. It provides comprehensive inventory management including stock levels, dashboards, and health analysis.

## Overview

This domain provides functionality for:
- Retrieving inventory with pagination
- Retrieving inventory by product
- Retrieving inventory by category
- Viewing inventory dashboard with key metrics
- Analyzing inventory health (overstock and zero-movement alerts)

Related documentation:
- See [ProductDomain](./ProductDomain.MD) for products
- See [StockMovementDomain](./StockMovementDomain.MD) for movements

## Business Logic

### Inventory Management
- Products are tracked with their quantity, cost price, and sales price
- Inventory can be viewed globally, by product, or by category
- Results include total cost value, sales value, and quantity

### Inventory Dashboard
The dashboard provides:
- **Low Stock Items**: Products with lowest quantity
- **Total Customers**: Number of registered customers
- **Total Employees**: Number of registered employees
- **Total Cost Value**: Sum of (cost price * quantity) for all products
- **Total Sales Value**: Sum of (sales price * quantity) for all products
- **Profit Potential**: Sales value - Cost value
- **Category Breakdown**: Inventory value grouped by category
- **Supplier Breakdown**: Inventory value grouped by supplier

### Inventory Health Analysis
The health endpoint provides:
- **Overstock Alerts**: Products with excess inventory based on sales velocity
- **Zero Movement Products**: Products with no stock movements or orders
- **Stock Value by Category**: Inventory value breakdown
- **Health Summary**: Total products, healthy products, percentages

### Overstock Detection
Products are flagged as overstock when:
- Current quantity exceeds (average monthly sales * overstock multiplier)
- Default multiplier is 3.0x

### Zero Movement Detection
Products are flagged as zero-movement when:
- No stock movements or orders in specified days
- Default threshold is 90 days

## Components

### Controllers

#### InventoryController
HTTP endpoint controller providing REST API for inventory operations.

- `GET /inventory` - Retrieves paginated inventory
- `GET /inventory/product/{productId}` - Retrieves inventory for a specific product
- `GET /inventory/category/{categoryId}` - Retrieves inventory for a category
- `GET /inventory/dashboard` - Retrieves inventory dashboard
- `GET /inventory/health` - Retrieves inventory health analysis

### Handlers

#### GetInventoryHandler
Handles retrieval of paginated inventory data with totals.

#### GetInventoryByProductHandler
Handles retrieval of inventory for a specific product.

#### GetInventoryByCategoryHandler
Handles retrieval of inventory for products in a specific category.

#### GetInventoryDashboardHandler
Handles generation of inventory dashboard with metrics and breakdowns.

#### GetInventoryHealthHandler
Handles generation of inventory health analysis including overstock and zero-movement alerts.

### Queries

#### GetInventoryQuery
Query for retrieving inventory with pagination.
- `Page`: Page number (default: 1)
- `PerPage`: Number of items per page (default: 10)

#### GetInventoryByProductQuery
Query for retrieving inventory by product ID.
- `ProductId`: Product ID to filter by
- `Page`: Page number (default: 1)
- `PerPage`: Number of items per page (default: 10)

#### GetInventoryByCategoryQuery
Query for retrieving inventory by category ID.
- `CategoryId`: Category ID to filter by
- `Page`: Page number (default: 1)
- `PerPage`: Number of items per page (default: 10)

#### GetInventoryHealthQuery
Query for generating inventory health analysis.
- `ZeroMovementDays`: Days threshold for zero movement (default: 90)
- `OverstockMultiplier`: Multiplier for overstock calculation (default: 3.0)

### Responses

#### InventoryResponse
Response model containing inventory data.
- `Items`: List of inventory detail items
- `TotalCostPrice`: Total cost price
- `TotalSalesPrice`: Total sales price
- `TotalQuantity`: Total quantity

#### InventoryDetailResponse
Response model for inventory item details.
- `Id`: Product ID
- `Name`: Product name
- `Quantity`: Current quantity
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `CategoryId`: Category ID
- `CategoryName`: Category name

#### InventoryDashboardResponse
Response model for inventory dashboard.
- `LowStockItems`: Low stock products
- `TotalCustomers`: Total customers
- `TotalEmployees`: Total employees
- `TotalCostValue`: Total cost value
- `TotalSalesValue`: Total sales value
- `TotalQuantity`: Total quantity
- `ProfitPotential`: Potential profit
- `CategoryBreakdown`: Breakdown by category
- `SupplierBreakdown`: Breakdown by supplier

#### InventoryHealthResponse
Response model for inventory health analysis.
- `OverstockAlert`: Overstock products alert
- `ZeroMovementProducts`: Products with no movement
- `StockValueByCategory`: Stock value by category
- `Summary`: Health summary

#### InventoryHealthSummaryResponse
Response model for health summary metrics.
- `TotalProducts`: Total products
- `HealthyProducts`: Healthy products count
- `OverstockProducts`: Overstock products count
- `ZeroMovementProducts`: Zero movement products count
- `TotalStockValue`: Total stock value
- `OverstockPercentage`: Overstock percentage
- `ZeroMovementPercentage`: Zero movement percentage

#### OverstockAlertResponse
Response model for overstock alerts.
- `TotalOverstockProducts`: Number of overstock products
- `TotalOverstockValue`: Total overstock value
- `Products`: List of overstock products

#### OverstockProductResponse
Response model for overstock product details.
- `ProductId`: Product ID
- `ProductName`: Product name
- `CategoryName`: Category name
- `CurrentQuantity`: Current stock quantity
- `RecommendedQuantity`: Recommended quantity
- `ExcessValue`: Excess value
- `CostPrice`: Cost price

#### ZeroMovementProductResponse
Response model for zero-movement products.
- `ProductId`: Product ID
- `ProductName`: Product name
- `CategoryName`: Category name
- `SupplierName`: Supplier name
- `CurrentStock`: Current stock
- `StockValue`: Stock value
- `LastMovementDate`: Last movement date
- `DaysWithoutMovement`: Days without movement

#### CategoryBreakdownResponse
Response model for category breakdown.
- `CategoryId`: Category ID
- `CategoryName`: Category name
- `TotalCostValue`: Total cost value
- `TotalSalesValue`: Total sales value
- `TotalQuantity`: Total quantity

#### SupplierBreakdownResponse
Response model for supplier breakdown.
- `SupplierId`: Supplier ID
- `SupplierName`: Supplier name
- `TotalCostValue`: Total cost value
- `TotalSalesValue`: Total sales value
- `TotalQuantity`: Total quantity

#### StockValueByCategoryResponse
Response model for stock value by category.
- `CategoryId`: Category ID
- `CategoryName`: Category name
- `ProductCount`: Number of products
- `TotalStockValue`: Total stock value
- `Percentage`: Percentage of total

#### InventoryDashboardItemResponse
Response model for dashboard item.
- `Id`: Product ID
- `Name`: Product name
- `Quantity`: Quantity
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `CategoryId`: Category ID
- `CategoryName`: Category name

## Security

- All inventory endpoints require authentication
- No additional authorization checks beyond authentication

## Testing

The Inventory domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/Inventory/`.

### Test Coverage

#### InventoryControllerTests
Tests the HTTP endpoint layer including:
- Inventory retrieval endpoints
- Dashboard endpoint
- Health analysis endpoint

#### GetInventoryHandlerTests
Tests inventory retrieval with pagination.

#### GetInventoryByProductHandlerTests
Tests inventory retrieval by product ID.

#### GetInventoryByCategoryHandlerTests
Tests inventory retrieval by category.
