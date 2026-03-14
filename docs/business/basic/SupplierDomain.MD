# Supplier Domain

The Supplier domain manages supplier records within a company. Suppliers provide products and are tracked for inventory management.

## Overview

This domain provides functionality for:
- Retrieving all suppliers with pagination
- Retrieving supplier details by ID
- Creating new suppliers
- Updating existing suppliers
- Deleting suppliers (soft delete)
- Retrieving supplier performance analytics

## Business Logic

### Supplier Entity
A supplier represents a company or individual that provides products with the following characteristics:
- **Id**: Unique identifier (GUID)
- **PersonId**: Associated person (contact information)
- **CNPJ**: Brazilian company tax identification number
- **Person**: Contact details (name, email, phone, address)
- **Deleted**: Soft delete timestamp

### Supplier Creation
- When a supplier is created, an associated Person record is also created
- Contact information includes: name, email, phone, document, address

### Supplier Updates
- Suppliers can be updated with new contact information or CNPJ

## Components

### Controllers

#### SupplierController
HTTP endpoint controller providing REST API for supplier operations.

- `GET /Supplier` - Retrieves paginated list of all suppliers
- `GET /Supplier/{id}` - Retrieves a specific supplier by ID
- `POST /Supplier` - Creates a new supplier
- `PATCH /Supplier/{id}` - Updates an existing supplier
- `DELETE /Supplier/{id}` - Deletes a supplier (soft delete)
- `GET /Supplier/performance` - Retrieves supplier performance analytics

### Handlers

#### GetAllSupplierHandler
Handles retrieval of all suppliers with pagination. Returns suppliers with contact information.

#### GetSupplierByIdHandler
Handles retrieval of a specific supplier by ID.

#### AddSupplierHandler
Handles supplier creation requests. Creates both supplier and person records.

#### UpdateSupplierHandler
Handles supplier update requests. Updates supplier and person information.

#### DeleteSupplierHandler
Handles supplier deletion (soft delete).

#### GetSupplierPerformanceHandler
Handles supplier performance analytics. Returns:
- Products per supplier
- Cost comparisons
- Recent stock movements
- Summary statistics

### Commands

#### AddSupplierCommand
Command record for creating a new supplier.
- `Id`: Unique identifier
- `Name`: Supplier name
- `Email`: Email address
- `Document`: Document number
- `City`: City
- `Complement`: Address complement
- `Neighborhood`: Neighborhood
- `Number`: Address number
- `StateId`: State ID
- `Street`: Street address
- `ZipCode`: Zip code
- `PhoneNumber`: Phone number
- `Cnpj`: CNPJ number

#### UpdateSupplierCommand
Command record for updating a supplier.
- `Id`: Supplier ID to update
- `Name`: Updated name
- `Email`: Updated email
- `Document`: Updated document
- `City`: Updated city
- `Complement`: Updated complement
- `Neighborhood`: Updated neighborhood
- `Number`: Updated number
- `StateId`: Updated state ID
- `Street`: Updated street
- `ZipCode`: Updated zip code
- `PhoneNumber`: Updated phone
- `Cnpj`: Updated CNPJ

#### DeleteSupplierCommand
Command record for deleting a supplier.
- `Id`: Supplier ID to delete

### Queries

#### GetAllSupplierQuery
Query for retrieving all suppliers with pagination.
- `Page`: Page number
- `PerPage`: Items per page

#### GetSupplierByIdQuery
Query for retrieving a supplier by ID.
- `Id`: Supplier ID

#### GetSupplierPerformanceQuery
Query for retrieving performance analytics.
- `Days`: Number of days to analyze
- `TopLimit`: Number of top suppliers

### Responses

#### GetAllSupplierResponse
Response model for a supplier in a list.
- `Id`: Supplier ID
- `PersonId`: Person ID
- `Name`: Supplier name
- `Email`: Email
- `PhoneNumber`: Phone
- `Document`: Document
- `Street`: Street
- `Number`: Number
- `Complement`: Complement
- `Neighborhood`: Neighborhood
- `ZipCode`: Zip code
- `StateId`: State ID
- `City`: City

#### GetSupplierByIdResponse
Response model for a single supplier.
- `Id`: Supplier ID
- `PersonId`: Person ID
- `Name`: Name
- `Email`: Email
- `PhoneNumber`: Phone
- `Document`: Document
- `Street`: Street
- `Number`: Number
- `Complement`: Complement
- `Neighborhood`: Neighborhood
- `ZipCode`: Zip code
- `StateId`: State ID
- `City`: City

#### AddSupplierResponse
Response for created supplier.
- `Id`: Supplier ID
- `Cnpj`: CNPJ

#### UpdateSupplierResponse
Response for updated supplier.
- `Id`: Supplier ID
- `Cnpj`: CNPJ

#### SupplierPerformanceResponse
Response containing performance analytics.
- `ProductsPerSupplier`: Product counts
- `CostComparison`: Cost analysis
- `RecentStockMovements`: Recent movements
- `Summary`: Summary statistics

#### SupplierSummaryResponse
Summary statistics.
- `TotalSuppliers`: Total count
- `TotalProducts`: Total products
- `TotalStockValue`: Total value
- `AverageProductsPerSupplier`: Average products

## Data Model

### SupplierModel
Entity representing a supplier in the database:
- Mapped to `basic.suppliers` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt, Deleted)
- Has relationships with: Person, Products, StockMovements

### PersonModel
Entity representing contact information:
- Mapped to `common.people` table
- Has relationships with: Suppliers, Customers, Employees

## Security

- All supplier endpoints require authentication

## Testing

The Supplier domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/Supplier/`.

### Test Coverage

#### SupplierControllerTests
Tests the HTTP endpoint layer including:
- Retrieving suppliers with pagination
- Retrieving supplier by ID
- Creating new suppliers
- Updating existing suppliers
- Deleting suppliers
- Controller attribute validation

#### AddSupplierHandlerTests
Tests supplier creation including:
- Supplier entity creation
- Person entity creation
- Database persistence

#### GetAllSupplierHandlerTests
Tests supplier retrieval with pagination.

#### GetSupplierByIdHandlerTests
Tests supplier retrieval by ID.

#### UpdateSupplierHandlerTests
Tests supplier update logic.

#### DeleteSupplierHandlerTests
Tests supplier deletion (soft delete).

#### GetSupplierPerformanceHandlerTests
Tests performance analytics including:
- Products per supplier calculations
- Stock movement analysis
- Cost comparisons
