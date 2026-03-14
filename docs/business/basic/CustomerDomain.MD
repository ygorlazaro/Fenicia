# Customer Domain

The Customer domain manages customer records within a company in the Fenicia system. It provides CRUD operations, customer data management, and analytics.

## Overview

This domain provides functionality for:
- Retrieving all customers with pagination
- Retrieving a specific customer by ID
- Creating new customers
- Updating existing customer information
- Deleting customers (soft delete)
- Generating customer analytics and insights

## Business Logic

### Customer Entity
A customer represents a person or business entity in the system with the following characteristics:
- **Id**: Unique identifier (GUID)
- **PersonId**: Reference to the associated person record
- **Person**: Associated person with name, email, document, phone, address details
- **Deleted**: Soft delete timestamp (null if not deleted)

### Customer Retrieval
- Customers can be retrieved with pagination support
- Results include all customer and person details
- Default pagination is 10 items per page

### Customer Creation
- Creates both customer and associated person records
- All address fields are optional
- Document and contact information can be provided

### Customer Updates
- Updates both customer and associated person records
- All fields can be modified
- Returns null if customer does not exist

### Customer Deletion (Soft Delete)
- Performs a soft delete by setting the Deleted timestamp
- Customer data is not physically removed from the database
- Deleted customers are filtered out from regular queries

### Customer Insights
The insights endpoint provides:
- **Summary**: Total customers, orders, revenue, average order value, customer lifetime value
- **Top Customers**: Customers ranked by total spending
- **Recent Orders**: Latest orders across all customers
- **Risk Alerts**: Customers at risk of churn based on inactivity

## Components

### Controllers

#### CustomerController
HTTP endpoint controller providing REST API for customer operations.

- `GET /customer` - Retrieves paginated list of all customers
- `GET /customer/{id}` - Retrieves a specific customer by ID
- `POST /customer` - Creates a new customer
- `PATCH /customer/{id}` - Updates an existing customer
- `DELETE /customer/{id}` - Deletes a customer (soft delete)
- `GET /customer/insights` - Retrieves customer analytics

### Handlers

#### GetAllCustomerHandler
Handles retrieval of all customers with pagination. Returns paginated results including customer and person details.

#### GetCustomerByIdHandler
Handles retrieval of a specific customer by ID. Returns customer details including associated person information.

#### AddCustomerHandler
Handles customer creation requests. Creates both customer and person records.

#### UpdateCustomerHandler
Handles customer update requests. Updates both customer and associated person records.

#### DeleteCustomerHandler
Handles customer deletion requests. Performs soft delete by setting the Deleted timestamp.

#### GetCustomerInsightsHandler
Handles generation of customer analytics. Provides business intelligence including summary statistics, top customers, recent orders, and risk alerts.

### Commands

#### AddCustomerCommand
Command record for creating a new customer.
- `Id`: Unique identifier for the customer
- `Name`: Customer's full name
- `Email`: Customer's email address
- `Document`: Customer's document number (e.g., CPF)
- `City`: City address
- `Complement`: Address complement
- `Neighborhood`: Neighborhood
- `Number`: Address number
- `StateId`: State reference ID
- `Street`: Street address
- `ZipCode`: ZIP/Postal code
- `PhoneNumber`: Phone number

#### UpdateCustomerCommand
Command record for updating customer information.
- `Id`: Customer ID to update
- `Name`: New customer name
- `Email`: New email address
- `Document`: New document number
- `City`: New city
- `Complement`: New address complement
- `Neighborhood`: New neighborhood
- `Number`: New address number
- `StateId`: New state reference ID
- `Street`: New street address
- `ZipCode`: New ZIP/Postal code
- `PhoneNumber`: New phone number

#### DeleteCustomerCommand
Command record for deleting a customer.
- `Id`: Customer ID to delete

### Queries

#### GetAllCustomerQuery
Query for retrieving customers with pagination.
- `Page`: Page number (default: 1)
- `PerPage`: Number of items per page (default: 10)

#### GetCustomerByIdQuery
Query for retrieving a specific customer.
- `Id`: Customer ID to retrieve

#### GetCustomerInsightsQuery
Query for generating customer analytics.
- `Days`: Number of days to analyze (default: 90)
- `TopLimit`: Number of top customers to return (default: 10)
- `RiskThresholdDays`: Days threshold for risk alerts (default: 60)

### Responses

#### GetAllCustomerResponse
Response model for customer list items.
- `Id`: Customer ID
- `PersonId`: Person ID
- `Name`: Customer name
- `Email`: Email address
- `PhoneNumber`: Phone number
- `Document`: Document number
- `Street`: Street address
- `Number`: Address number
- `Complement`: Address complement
- `Neighborhood`: Neighborhood
- `ZipCode`: ZIP code
- `StateId`: State ID
- `City`: City

#### GetCustomerByIdResponse
Response model for customer details.
- `Id`: Customer ID
- `PersonId`: Person ID
- `Name`: Customer name
- `Email`: Email address
- `PhoneNumber`: Phone number
- `Document`: Document number
- `Street`: Street address
- `Number`: Address number
- `Complement`: Address complement
- `Neighborhood`: Neighborhood
- `ZipCode`: ZIP code
- `StateId`: State ID
- `City`: City

#### AddCustomerResponse
Response model returned after creating a customer.
- `Id`: Created customer ID
- `PersonId`: Created person ID

#### UpdateCustomerResponse
Response model returned after updating a customer.
- `Id`: Updated customer ID
- `PersonId`: Updated person ID

#### CustomerInsightsResponse
Response model containing customer analytics.
- `Summary`: Customer summary statistics
- `TopCustomers`: List of top customers by spending
- `RecentOrders`: List of recent orders
- `AtRiskCustomers`: List of customers at risk of churn

#### CustomerSummaryResponse
Response model containing aggregate statistics.
- `TotalCustomers`: Total number of customers
- `TotalOrders`: Total number of orders
- `TotalRevenue`: Total revenue generated
- `AverageOrderValue`: Average order value
- `AverageCustomerLifetimeValue`: Average customer lifetime value

#### CustomerOrderHistoryResponse
Response model for customer order history.
- `CustomerId`: Customer ID
- `CustomerName`: Customer name
- `OrderCount`: Total number of orders
- `TotalSpent`: Total amount spent
- `TotalItems`: Total items purchased
- `FirstOrderDate`: Date of first order
- `LastOrderDate`: Date of last order
- `AverageOrderValue`: Average order value

#### CustomerRecentOrdersResponse
Response model for recent orders.
- `OrderId`: Order ID
- `CustomerId`: Customer ID
- `CustomerName`: Customer name
- `TotalAmount`: Order total amount
- `SaleDate`: Sale date
- `Status`: Order status
- `TotalItems`: Total items in order

#### CustomerRiskAlertResponse
Response model for customer risk alerts.
- `CustomerId`: Customer ID
- `CustomerName`: Customer name
- `PreviousOrderCount`: Previous order count
- `LastOrderDate`: Date of last order
- `DaysSinceLastOrder`: Days since last order
- `PreviousTotalSpent`: Total previously spent
- `RiskLevel`: Risk level (Low, Medium, High)

## Data Model

### CustomerModel
Entity representing a customer in the database:
- Mapped to `basic.customers` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt)
- Has a relationship with: PersonModel

### PersonModel
Entity representing a person's information:
- Mapped to `basic.persons` table
- Contains: Name, Email, Document, PhoneNumber, Address fields

## Security

- All customer endpoints require authentication
- Users can only access customer data they are authorized to view
- WideEventContext is used for request tracking and logging

## Testing

The Customer domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/Customer/`.

### Test Coverage

#### CustomerControllerTests
Tests the HTTP endpoint layer including:
- Retrieving customers with pagination
- Retrieving customer by ID
- Creating new customers
- Updating existing customers
- Deleting customers
- Controller attribute validation (Authorize, Route, ApiController)

**Key test scenarios:**
- Empty customer list returns valid empty pagination
- Customer list returns paginated results
- Customer by ID returns correct customer
- Non-existent customer returns NotFound
- Customer creation returns Created result
- Customer update returns Ok with updated data
- Non-existent customer update returns NotFound
- Customer deletion returns NoContent
- Deleting non-existent customer returns NoContent

#### GetAllCustomerHandlerTests
Tests customer list retrieval logic including:
- Empty database handling
- Multiple customers retrieval
- Pagination (page boundaries, empty pages)
- Default pagination values

**Key test scenarios:**
- Empty database returns empty list
- All customers are returned correctly
- Pagination returns correct page of results
- Page beyond available returns empty list
- Default pagination returns first 10 items

#### GetCustomerByIdHandlerTests
Tests customer retrieval by ID logic including:
- Customer existence checks
- Multiple customer handling
- Null address field handling

**Key test scenarios:**
- Existing customer returns correct data
- Non-existent customer returns null
- Empty database returns null
- Only requested customer is returned
- Null address fields are handled correctly

#### AddCustomerHandlerTests
Tests customer creation logic including:
- Valid customer creation
- Null field handling
- Database persistence

**Key test scenarios:**
- Customer is created successfully
- Null phone number is handled
- Null street is handled
- Null zip code is handled
- Null number is handled
- Null complement is preserved
- Null neighborhood is preserved
- Null city is preserved
- Customer is persisted to database
- Multiple customers can be created

#### UpdateCustomerHandlerTests
Tests customer update logic including:
- Customer existence checks
- Field update validation
- Database persistence

**Key test scenarios:**
- Customer is updated successfully
- Non-existent customer returns null
- Empty database returns null
- Null phone number is handled
- Null street is handled
- Null zip code is handled
- Null number is handled
- Null complement is preserved
- Null neighborhood is preserved
- Null city is preserved
- Customer update is persisted to database

#### DeleteCustomerHandlerTests
Tests customer deletion (soft delete) logic including:
- Soft delete functionality
- Multiple customer handling
- Empty database handling

**Key test scenarios:**
- Existing customer is soft deleted
- Deleted timestamp is set correctly
- Non-existent customer does nothing
- Only specified customer is deleted
- Empty database does nothing
