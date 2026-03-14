# Position Domain

## Overview

The Position domain manages employee positions/roles within a company. Positions categorize employees by their job function (e.g., Manager, Sales, Support, Developer).

Related documentation:
- See [EmployeeDomain](./EmployeeDomain.MD) for employees

## Architecture

### Controller

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/Position` | GET | Get all positions with pagination |
| `/Position/{id}` | GET | Get position by ID |
| `/Position/{id}/employee` | GET | Get employees in a position |
| `/Position` | POST | Create a new position |
| `/Position/{id}` | PATCH | Update an existing position |
| `/Position/{id}` | DELETE | Delete a position (soft delete) |

### Handlers

| Handler | Description |
|---------|-------------|
| `GetAllPositionHandler` | Retrieves all positions with pagination |
| `GetPositionByIdHandler` | Retrieves a position by its ID |
| `AddPositionHandler` | Creates a new position |
| `UpdatePositionHandler` | Updates an existing position |
| `DeletePositionHandler` | Soft-deletes a position |

### Commands

| Command | Description |
|---------|-------------|
| `AddPositionCommand` | Request to create a new position |
| `UpdatePositionCommand` | Request to update an existing position |
| `DeletePositionCommand` | Request to delete a position |

### Queries

| Query | Description |
|-------|-------------|
| `GetAllPositionQuery` | Request to get all positions with pagination |
| `GetPositionByIdQuery` | Request to get a position by ID |

### Responses

| Response | Description |
|----------|-------------|
| `GetAllPositionResponse` | Paginated list of positions |
| `GetPositionByIdResponse` | Single position details |
| `AddPositionResponse` | Created position data |
| `UpdatePositionResponse` | Updated position data |

## Usage

Positions are used to categorize employees by their job function. Each position has:
- **Id** - Unique identifier
- **Name** - Position name (e.g., "Manager", "Sales", "Support")

Positions are linked to employees via the `PositionId` field in the Employee entity.

## Data Model

The `PositionModel` entity stores:
- Unique identifier (Id)
- Position name (Name)
- Soft delete timestamp (Deleted)

Positions can be soft-deleted, which sets the Deleted timestamp rather than removing the record.
