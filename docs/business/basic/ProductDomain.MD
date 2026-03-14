# Product Domain

The Product domain manages product catalog within a company. Products are items that can be sold, tracked in inventory, and associated with categories and suppliers.

## Overview

This domain provides functionality for:
- Retrieving all products with pagination
- Retrieving products by category
- Retrieving product details by ID
- Creating new products
- Updating existing products
- Deleting products (soft delete)
- Analyzing product performance metrics

## Business Logic

### Product Entity
A product represents an item in the company's catalog with the following characteristics:
- **Id**: Unique identifier (GUID)
- **Name**: Product name (required)
- **CostPrice**: Cost price for the product (optional)
- **SalesPrice**: Sales price for the product (required)
- **Quantity**: Current stock quantity
- **CategoryId**: Associated product category (required)
- **SupplierId**: Associated supplier (optional)
- **Deleted**: Soft delete timestamp

### Product Retrieval
- Products can be retrieved with pagination
- Results include category and supplier information
- Products are filtered by soft delete status (Deleted = null)

### Product Updates
- Products can be updated with new prices, quantities, category, or supplier
- Updates preserve the product ID

### Product Performance
- Best-selling products analysis
- Worst-selling products analysis
- Profit margin calculations
- Products that have never been sold

## Components

### Controllers

#### ProductController
HTTP endpoint controller providing REST API for product operations.

- `GET /Product` - Retrieves paginated list of all products
- `GET /Product/{id}` - Retrieves a specific product by ID
- `POST /Product` - Creates a new product
- `PATCH /Product/{id}` - Updates an existing product
- `DELETE /Product/{id}` - Deletes a product (soft delete)
- `GET /Product/performance` - Retrieves product performance metrics

### Handlers

#### GetAllProductHandler
Handles retrieval of all products with pagination. Returns paginated results including category and supplier details.

#### GetProductByIdHandler
Handles retrieval of a specific product by ID. Returns product details including category and supplier information.

#### GetProductsByCategoryIdHandler
Handles retrieval of products by category ID. Returns paginated list of products in a specific category.

#### AddProductHandler
Handles product creation requests. Creates a new product in the database.

#### UpdateProductHandler
Handles product update requests. Updates product information including name, prices, quantity, category, and supplier.

#### DeleteProductHandler
Handles product deletion (soft delete). Sets the Deleted timestamp to mark the product as removed.

#### GetProductPerformanceHandler
Handles product performance analysis requests. Returns comprehensive analytics including:
- Best-selling products
- Worst-selling products
- Profit margin analysis
- Products that have never been sold

### Commands

#### AddProductCommand
Command record for creating a new product.
- `Id`: Unique identifier for the new product
- `Name`: Name of the product
- `CostPrice`: Cost price of the product
- `SalesPrice`: Sales price of the product
- `Quantity`: Quantity in stock
- `CategoryId`: Category ID the product belongs to
- `SupplierId`: Supplier ID (optional)

#### UpdateProductCommand
Command record for updating an existing product.
- `Id`: Unique identifier of the product to update
- `Name`: Updated name of the product
- `CostPrice`: Updated cost price
- `SalesPrice`: Updated sales price
- `Quantity`: Updated quantity
- `CategoryId`: Updated category ID
- `SupplierId`: Updated supplier ID (optional)

#### DeleteProductCommand
Command record for deleting a product.
- `Id`: Unique identifier of the product to delete

### Queries

#### GetAllProductQuery
Query for retrieving all products with pagination.
- `Page`: Page number
- `PerPage`: Number of items per page

#### GetProductByIdQuery
Query for retrieving a product by ID.
- `Id`: Product ID

#### GetProductsByCategoryIdQuery
Query for retrieving products by category.
- `CategoryId`: Category ID to filter by
- `Page`: Page number
- `PerPage`: Number of items per page

#### GetProductPerformanceQuery
Query for retrieving product performance metrics.
- `Days`: Number of days to analyze
- `TopLimit`: Number of top products to return

### Responses

#### GetAllProductResponse
Response model for a product in a list.
- `Id`: Product ID
- `Name`: Product name
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `Quantity`: Quantity in stock
- `CategoryId`: Category ID
- `CategoryName`: Category name
- `SupplierId`: Supplier ID
- `SupplierName`: Supplier name

#### GetProductByIdResponse
Response model for retrieving a single product.
- `Id`: Product ID
- `Name`: Product name
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `Quantity`: Quantity in stock
- `CategoryId`: Category ID
- `CategoryName`: Category name
- `SupplierId`: Supplier ID
- `SupplierName`: Supplier name

#### AddProductResponse
Response model for a newly created product.
- `Id`: Product ID
- `Name`: Product name
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `Quantity`: Quantity
- `CategoryId`: Category ID
- `CategoryName`: Category name
- `SupplierId`: Supplier ID
- `SupplierName`: Supplier name

#### UpdateProductResponse
Response model for an updated product.
- `Id`: Product ID
- `Name`: Product name
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `Quantity`: Quantity
- `CategoryId`: Category ID
- `CategoryName`: Category name
- `SupplierId`: Supplier ID
- `SupplierName`: Supplier name

#### ProductPerformanceResponse
Response model containing product performance metrics.
- `BestSellingProducts`: List of best-selling products
- `WorstSellingProducts`: List of worst-selling products
- `ProfitMargins`: List of products with profit margin analysis
- `NeverSoldProducts`: List of products that have never been sold

#### BestSellingProductResponse
Response model for a best-selling product.
- `ProductId`: Product ID
- `ProductName`: Product name
- `CategoryName`: Category name
- `TotalQuantitySold`: Total quantity sold
- `TotalRevenue`: Total revenue generated
- `OrderCount`: Number of orders
- `AveragePrice`: Average price per unit

#### WorstSellingProductResponse
Response model for a worst-selling product.
- `ProductId`: Product ID
- `ProductName`: Product name
- `CategoryName`: Category name
- `TotalQuantitySold`: Total quantity sold
- `TotalRevenue`: Total revenue generated
- `OrderCount`: Number of orders
- `CurrentStock`: Current stock quantity
- `CostValue`: Total cost value

#### ProfitMarginResponse
Response model for profit margin analysis.
- `ProductId`: Product ID
- `ProductName`: Product name
- `CategoryName`: Category name
- `CostPrice`: Cost price
- `SalesPrice`: Sales price
- `ProfitMargin`: Profit margin percentage
- `MarginClassification`: Classification (Excellent, Good, Average, Low, Very Low)

#### NeverSoldProductResponse
Response model for products never sold.
- `ProductId`: Product ID
- `ProductName`: Product name
- `CategoryName`: Category name
- `SupplierName`: Supplier name
- `CurrentStock`: Current stock quantity
- `CostValue`: Total cost value
- `LastStockMovement`: Last stock movement date

## Data Model

### ProductModel
Entity representing a product in the database:
- Mapped to `basic.products` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt, Deleted)
- Has relationships with: Category (ProductCategoryModel), Supplier (SupplierModel), StockMovements, OrderDetails

## Security

- All product endpoints require authentication
- Product creation requires valid category and supplier references
- Soft delete preserves product history

## Testing

The Product domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/Product/`.

### Test Coverage

#### ProductControllerTests
Tests the HTTP endpoint layer including:
- Retrieving products with pagination
- Retrieving product by ID
- Creating new products
- Updating existing products
- Deleting products
- WideEventContext propagation for request tracking
- Controller attribute validation (Authorize, Route, ApiController)

**Key test scenarios:**
- Empty product list returns valid empty pagination
- Products with categories return paginated results
- Creating product with valid command returns Created result
- Updating product returns Ok with updated details
- Non-existent product returns NotFound
- Deleting existing product returns NoContent

#### AddProductHandlerTests
Tests the business logic layer for product creation including:
- Product entity creation
- Database persistence
- Response mapping

**Key test scenarios:**
- Valid command creates product successfully
- Product is saved to database correctly

#### GetAllProductHandlerTests
Tests product retrieval logic including:
- Pagination handling
- Category and supplier joins

#### UpdateProductHandlerTests
Tests product update logic including:
- Product existence validation
- Field updates

#### DeleteProductHandlerTests
Tests product deletion (soft delete) including:
- Soft delete timestamp setting

#### GetProductPerformanceHandlerTests
Tests product performance analytics including:
- Best-selling products calculation
- Worst-selling products calculation
- Profit margin classification
- Never sold products identification
