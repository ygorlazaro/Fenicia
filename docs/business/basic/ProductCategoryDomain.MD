# ProductCategory Domain

The ProductCategory domain manages product categories within a company. Categories are used to organize and classify products.

## Overview

This domain provides functionality for:
- Retrieving all categories with pagination
- Retrieving category details by ID
- Creating new categories
- Updating existing categories
- Deleting categories (soft delete)
- Retrieving products by category

## Business Logic

### ProductCategory Entity
A product category represents a classification for products with the following characteristics:
- **Id**: Unique identifier (GUID)
- **Name**: Category name (required)
- **Deleted**: Soft delete timestamp

### Category Retrieval
- Categories can be retrieved with pagination
- Results include category names only

### Category Updates
- Categories can be updated with a new name

## Components

### Controllers

#### ProductCategoryController
HTTP endpoint controller providing REST API for category operations.

- `GET /ProductCategory` - Retrieves paginated list of all categories
- `GET /ProductCategory/{id}` - Retrieves a specific category by ID
- `POST /ProductCategory` - Creates a new category
- `PATCH /ProductCategory/{id}` - Updates an existing category
- `DELETE /ProductCategory/{id}` - Deletes a category (soft delete)
- `GET /ProductCategory/{id}/product` - Retrieves products in a category

### Handlers

#### GetAllProductCategoryHandler
Handles retrieval of all categories with pagination.

#### GetProductByIdHandler
Handles retrieval of a specific category by ID.

#### AddProductCategoryHandler
Handles category creation requests.

#### UpdateProductCategoryHandler
Handles category update requests.

#### DeleteProductCategoryHandler
Handles category deletion (soft delete).

### Commands

#### AddProductCategoryCommand
Command record for creating a new category.
- `Id`: Unique identifier for the new category
- `Name`: Name of the category

#### UpdateProductCategoryCommand
Command record for updating an existing category.
- `Id`: Unique identifier of the category to update
- `Name`: Updated name of the category

#### DeleteProductCategoryCommand
Command record for deleting a category.
- `Id`: Unique identifier of the category to delete

### Queries

#### GetAllProductCategoryQuery
Query for retrieving all categories with pagination.
- `Page`: Page number
- `PerPage`: Number of items per page

#### GetProductCategoryByIdQuery
Query for retrieving a category by ID.
- `Id`: Category ID

### Responses

#### GetAllProductCategoryResponse
Response model for a category in a list.
- `Id`: Category ID
- `Name`: Category name

#### GetProductCategoryByIdResponse
Response model for retrieving a single category.
- `Id`: Category ID
- `Name`: Category name

#### AddProductCategoryResponse
Response model for a newly created category.
- `Id`: Category ID
- `Name`: Category name

#### UpdateProductCategoryResponse
Response model for an updated category.
- `Id`: Category ID
- `Name`: Category name

## Data Model

### ProductCategoryModel
Entity representing a product category in the database:
- Mapped to `basic.product_categories` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt, Deleted)
- Has relationships with: Products

## Security

- All category endpoints require authentication
- Categories can be retrieved by any authenticated user

## Testing

The ProductCategory domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/ProductCategory/`.

### Test Coverage

#### ProductCategoryControllerTests
Tests the HTTP endpoint layer including:
- Retrieving categories with pagination
- Retrieving category by ID
- Creating new categories
- Updating existing categories
- Deleting categories
- Controller attribute validation (Authorize, Route, ApiController)

#### AddProductCategoryHandlerTests
Tests the business logic layer for category creation.

#### GetAllProductCategoryHandlerTests
Tests category retrieval logic including pagination.

#### GetProductCategoryByIdHandlerTests
Tests category retrieval by ID.

#### UpdateProductCategoryHandlerTests
Tests category update logic.

#### DeleteProductCategoryHandlerTests
Tests category deletion (soft delete).
