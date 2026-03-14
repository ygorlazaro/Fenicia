# Employee Domain

The Employee domain manages employee records within a company in the Fenicia system. It provides CRUD operations, employee data management, and performance analytics.

## Overview

This domain provides functionality for:
- Retrieving all employees with pagination
- Retrieving a specific employee by ID
- Retrieving employees filtered by position
- Creating new employees
- Updating existing employee information
- Deleting employees (soft delete)
- Generating employee performance analytics

Related documentation:
- See [OrderDomain](./OrderDomain.MD) for order management

## Business Logic

### Employee Entity
An employee represents a person who works for the company with the following characteristics:
- **Id**: Unique identifier (GUID)
- **PositionId**: Reference to the employee's position
- **PersonId**: Reference to the associated person record
- **Person**: Associated person with name, email, document, phone, address details
- **Deleted**: Soft delete timestamp (null if not deleted)

### Employee Retrieval
- Employees can be retrieved with pagination support
- Results include all employee, person, and position details
- Default pagination is 10 items per page

### Employee Creation
- Creates both employee and associated person records
- All address fields are optional
- Document and contact information can be provided

### Employee Updates
- Updates both employee and associated person records
- All fields can be modified
- Returns null if employee does not exist

### Employee Deletion (Soft Delete)
- Performs a soft delete by setting the Deleted timestamp
- Employee data is not physically removed from the database
- Deleted employees are filtered out from regular queries

### Employee Performance
The performance endpoint provides:
- **Summary**: Total employees, active employees, total sales, total orders, averages
- **Sales by Employee**: Sales metrics grouped by employee
- **Orders by Employee**: Order counts grouped by employee
- **Top Performers**: Highest performing employees ranked by sales with performance levels (Standard, Good, Very Good, Excellent)

## Components

### Controllers

#### EmployeeController
HTTP endpoint controller providing REST API for employee operations.

- `GET /employee` - Retrieves paginated list of all employees
- `GET /employee/{id}` - Retrieves a specific employee by ID
- `POST /employee` - Creates a new employee
- `PATCH /employee/{id}` - Updates an existing employee
- `DELETE /employee/{id}` - Deletes an employee (soft delete)
- `GET /employee/performance` - Retrieves employee performance analytics

### Handlers

#### GetAllEmployeeHandler
Handles retrieval of all employees with pagination. Returns paginated results including employee, person, and position details.

#### GetEmployeeByIdHandler
Handles retrieval of a specific employee by ID. Returns employee details including associated person information.

#### GetEmployeesByPositionIdHandler
Handles retrieval of employees filtered by position ID with pagination.

#### AddEmployeeHandler
Handles employee creation requests. Creates both employee and person records.

#### UpdateEmployeeHandler
Handles employee update requests. Updates both employee and associated person records.

#### DeleteEmployeeHandler
Handles employee deletion requests. Performs soft delete by setting the Deleted timestamp.

#### GetEmployeePerformanceHandler
Handles generation of employee performance analytics. Provides business intelligence including summary statistics, sales by employee, order counts, and top performers.

### Commands

#### AddEmployeeCommand
Command record for creating a new employee.
- `Id`: Unique identifier for the employee
- `PositionId`: Employee's position ID
- `Name`: Employee's full name
- `Email`: Employee's email address
- `Document`: Employee's document number
- `City`: City address
- `Complement`: Address complement
- `Neighborhood`: Neighborhood
- `Number`: Address number
- `StateId`: State reference ID
- `Street`: Street address
- `ZipCode`: ZIP/Postal code
- `PhoneNumber`: Phone number

#### UpdateEmployeeCommand
Command record for updating employee information.
- `Id`: Employee ID to update
- `PositionId`: New position ID
- `Name`: New employee name
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

#### DeleteEmployeeCommand
Command record for deleting an employee.
- `Id`: Employee ID to delete

### Queries

#### GetAllEmployeeQuery
Query for retrieving employees with pagination.
- `Page`: Page number (default: 1)
- `PerPage`: Number of items per page (default: 10)

#### GetEmployeeByIdQuery
Query for retrieving a specific employee.
- `Id`: Employee ID to retrieve

#### GetEmployeesByPositionIdQuery
Query for retrieving employees filtered by position.
- `PositionId`: Position ID to filter by
- `Page`: Page number (default: 1)
- `PerPage`: Number of items per page (default: 10)

#### GetEmployeePerformanceQuery
Query for generating employee performance analytics.
- `Days`: Number of days to analyze (default: 90)
- `TopLimit`: Number of top performers to return (default: 10)

### Responses

#### GetAllEmployeeResponse
Response model for employee list items.
- `Id`: Employee ID
- `PositionId`: Position ID
- `PersonId`: Person ID
- `Name`: Employee name
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
- `PositionName`: Position name
- `StateName`: State name

#### GetEmployeeByIdResponse
Response model for employee details.
- `Id`: Employee ID
- `PositionId`: Position ID
- `PersonId`: Person ID
- `Name`: Employee name
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

#### AddEmployeeResponse
Response model returned after creating an employee.
- `Id`: Created employee ID
- `PositionId`: Position ID
- `PersonId`: Created person ID

#### UpdateEmployeeResponse
Response model returned after updating an employee.
- `Id`: Updated employee ID
- `PositionId`: Position ID
- `PersonId`: Person ID

#### GetEmployeesByPositionIdResponse
Response model for employees filtered by position.
- `Id`: Employee ID
- `PositionId`: Position ID
- `PersonId`: Person ID

#### EmployeePerformanceResponse
Response model containing employee performance analytics.
- `Summary`: Performance summary statistics
- `SalesByEmployee`: Sales data grouped by employee
- `OrdersByEmployee`: Order counts grouped by employee
- `TopPerformers`: Top performing employees

#### EmployeePerformanceSummaryResponse
Response model containing aggregate performance statistics.
- `TotalEmployees`: Total number of employees
- `ActiveEmployees`: Number of employees with orders
- `TotalSales`: Total sales amount
- `TotalOrders`: Total number of orders
- `AverageSalesPerEmployee`: Average sales per employee
- `AverageOrdersPerEmployee`: Average orders per employee

#### EmployeeSalesResponse
Response model for employee sales data.
- `EmployeeId`: Employee ID
- `EmployeeName`: Employee name
- `PositionName`: Position name
- `TotalSales`: Total sales amount
- `TotalOrders`: Total number of orders
- `AverageOrderValue`: Average order value
- `Rank`: Sales rank

#### EmployeeOrderCountResponse
Response model for employee order counts.
- `EmployeeId`: Employee ID
- `EmployeeName`: Employee name
- `PositionName`: Position name
- `OrderCount`: Number of orders
- `TotalValue`: Total order value
- `FirstOrderDate`: Date of first order
- `LastOrderDate`: Date of last order

#### TopPerformerResponse
Response model for top performing employees.
- `EmployeeId`: Employee ID
- `EmployeeName`: Employee name
- `PositionName`: Position name
- `TotalSales`: Total sales amount
- `TotalOrders`: Total number of orders
- `PerformanceLevel`: Performance level (Standard, Good, Very Good, Excellent)

## Data Model

### EmployeeModel
Entity representing an employee in the database:
- Mapped to `basic.employees` table
- Inherits from `BaseModel` (includes Id, CreatedAt, UpdatedAt)
- Has relationships with: PersonModel, PositionModel

### PersonModel
Entity representing a person's information:
- Mapped to `basic.persons` table
- Contains: Name, Email, Document, PhoneNumber, Address fields

## Security

- All employee endpoints require authentication
- Users can only access employee data they are authorized to view
- WideEventContext is used for request tracking and logging

## Testing

The Employee domain has comprehensive unit tests located in `Fenicia.Module.Basic.Tests/Domains/Employee/`.

### Test Coverage

#### EmployeeControllerTests
Tests the HTTP endpoint layer including:
- Retrieving employees with pagination
- Retrieving employee by ID
- Creating new employees
- Updating existing employees
- Deleting employees
- Controller attribute validation (Authorize, Route, ApiController)

**Key test scenarios:**
- Empty employee list returns valid empty pagination
- Employee list returns paginated results
- Employee by ID returns correct employee
- Non-existent employee returns NotFound
- Employee creation returns Created result
- Employee update returns Ok with updated data
- Non-existent employee update returns NotFound
- Employee deletion returns NoContent
- Deleting non-existent employee returns NoContent

#### AddEmployeeHandlerTests
Tests employee creation logic.

#### GetAllEmployeeHandlerTests
Tests employee list retrieval logic with pagination.

#### GetEmployeeByIdHandlerTests
Tests employee retrieval by ID logic.

#### UpdateEmployeeHandlerTests
Tests employee update logic.

#### DeleteEmployeeHandlerTests
Tests employee deletion (soft delete) logic.

#### GetEmployeesByPositionIdHandlerTests
Tests employee retrieval filtered by position.
