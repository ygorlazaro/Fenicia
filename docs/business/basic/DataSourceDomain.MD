# DataSource Domain

The DataSource domain provides read-only endpoints for retrieving lists of entities for use as data sources in dropdowns and selection components in the Fenicia system.

## Overview

This domain provides functionality for:
- Retrieving all positions for dropdown selection
- Retrieving all product categories for dropdown selection
- Retrieving all suppliers for dropdown selection
- Retrieving all customers for dropdown selection
- Retrieving all products for dropdown selection
- Retrieving all employees for dropdown selection

## Business Logic

### DataSource Endpoints
All datasource endpoints return ordered lists of entities containing only ID and Name fields. These are designed for:
- Dropdown menus in forms
- Select components in UI
- Quick lookup of entity references

### Data Ordering
- All lists are ordered alphabetically by name
- For entities with Person relationship (customers, suppliers, employees), ordering is by person name

### Performance Considerations
- Customer and Employee handlers use `AsNoTracking()` for read-only query optimization
- Results are returned as simple lists (not paginated) since datasource lists are typically small

## Components

### Controllers

#### DataSourceController
HTTP endpoint controller providing REST API for datasource operations.

- `GET /datasource/position` - Retrieves all positions
- `GET /datasource/productcategory` - Retrieves all product categories
- `GET /datasource/supplier` - Retrieves all suppliers
- `GET /datasource/customer` - Retrieves all customers
- `GET /datasource/product` - Retrieves all products
- `GET /datasource/employee` - Retrieves all employees

### Handlers

#### GetAllPositionForDataSourceHandler
Retrieves all positions ordered by name.

#### GetAllProductCategoryForDataSourceHandler
Retrieves all product categories ordered by name.

#### GetAllSupplierForDataSourceHandler
Retrieves all suppliers ordered by person name.

#### GetAllCustomerForDataSourceHandler
Retrieves all customers ordered by person name. Uses AsNoTracking for read-only optimization.

#### GetAllProductForDataSourceHandler
Retrieves all products ordered by name.

#### GetAllEmployeeForDataSourceHandler
Retrieves all employees ordered by person name. Uses AsNoTracking for read-only optimization.

### Responses

#### GetAllPositionForDataSourceResponse
- `Id`: Position ID
- `Name`: Position name

#### GetAllProductCategoryForDataSourceResponse
- `Id`: Product category ID
- `Name`: Product category name

#### GetAllSupplierForDataSourceResponse
- `Id`: Supplier ID
- `Name`: Supplier (person) name

#### GetAllCustomerForDataSourceResponse
- `Id`: Customer ID
- `Name`: Customer (person) name

#### GetAllProductForDataSourceResponse
- `Id`: Product ID
- `Name`: Product name

#### GetAllEmployeeForDataSourceResponse
- `Id`: Employee ID
- `Name`: Employee (person) name

## Security

- All datasource endpoints require authentication
- WideEventContext is used for request tracking and logging
- No authorization checks beyond authentication (all authenticated users can access)

## Testing

The DataSource domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/DataSource/`.

### Test Coverage

#### DataSourceControllerTests
Tests the HTTP endpoint layer including:
- Retrieving positions, product categories, suppliers, customers, products, and employees
- Empty list handling
- Controller attribute validation (Authorize, Route, ApiController)

**Key test scenarios:**
- Empty position list returns valid empty list
- Position list returns positions
- Positions are ordered alphabetically by name
- Empty product category list returns valid empty list
- Product category list returns categories
- Empty supplier list returns valid empty list
- Supplier list returns suppliers
- Empty customer list returns valid empty list
- Customer list returns customers
- Empty product list returns valid empty list
- Product list returns products
- Empty employee list returns valid empty list
- Employee list returns employees
