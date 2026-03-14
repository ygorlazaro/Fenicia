# Order Domain (Basic Module)

## Overview

The Basic module's Order domain handles product orders within a company. It provides full CRUD operations and analytics capabilities.

This is an internal module that companies use to manage their own product orders, distinct from the platform-level module subscription orders.

Related documentation:
- See [CustomerDomain](./CustomerDomain.MD) for customer management
- See [EmployeeDomain](./EmployeeDomain.MD) for employee management
- See [ProductDomain](./ProductDomain.MD) for product information
- See [StockDomain](./StockDomain.MD) for inventory management

## Architecture

### Controllers

**OrderController**
- Location: `Fenicia.Module.Basic/Domains/Order/OrderController.cs`

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/order` | List all orders (paginated) |
| GET | `/order/{id}` | Get order by ID |
| POST | `/order` | Create new order |
| DELETE | `/order/{id}` | Delete order (Admin only, soft delete) |
| GET | `/order/{id}/detail` | Get order details |
| GET | `/order/analytics` | Get order analytics |

### Handlers

| Handler | Location | Description |
|---------|----------|-------------|
| `GetAllOrderHandler` | `Handlers/GetAllOrderHandler.cs` | Retrieves paginated orders |
| `GetOrderByIdHandler` | `Handlers/GetOrderByIdHandler.cs` | Retrieves order with details |
| `CreateOrderHandler` | `Handlers/CreateOrderHandler.cs` | Creates order and updates inventory |
| `DeleteOrderHandler` | `Handlers/DeleteOrderHandler.cs` | Soft-deletes order |
| `GetOrderAnalyticsHandler` | `Handlers/GetOrderAnalyticsHandler.cs` | Generates analytics |

### Queries

| Query | Location | Description |
|-------|----------|-------------|
| `GetAllOrderQuery` | `Queries/GetAllOrderQuery.cs` | Paginated order list query |
| `GetOrderByIdQuery` | `Queries/GetOrderByIdQuery.cs` | Single order query |
| `GetOrderAnalyticsQuery` | `Queries/GetOrderAnalyticsQuery.cs` | Analytics query with days and top customers limit |

### Commands

| Command | Location | Description |
|---------|----------|-------------|
| `CreateOrderCommand` | `Commands/CreateOrderCommand.cs` | Creates new product order |
| `DeleteOrderCommand` | `Commands/DeleteOrderCommand.cs` | Soft-deletes order |

### Responses

| Response | Location | Description |
|----------|----------|-------------|
| `CreateOrderResponse` | `Responses/CreateOrderResponse.cs` | Created order info |
| `GetAllOrderResponse` | `Responses/GetAllOrderResponse.cs` | Order summary for lists |
| `GetOrderByIdResponse` | `Responses/GetOrderByIdResponse.cs` | Order with full details |
| `OrderAnalyticsResponse` | `Responses/OrderAnalyticsResponse.cs` | Comprehensive analytics |
| `OrderDetailResponse` | `Responses/OrderDetailResponse.cs` | Single order item |
| `AverageOrderValueResponse` | `Responses/AverageOrderValueResponse.cs` | Order value statistics |
| `TopCustomerResponse` | `Responses/TopCustomerResponse.cs` | Top customer by spending |
| `SalesTrendResponse` | `Responses/SalesTrendResponse.cs` | Daily sales data |
| `OrderStatusCountResponse` | `Responses/OrderStatusCountResponse.cs` | Orders grouped by status |
| `CancelledOrderResponse` | `Responses/CancelledOrderResponse.cs` | Cancelled order info |

## Endpoint Details

### GET /order
Retrieves paginated list of orders.

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `perPage` (int, default: 10) - Items per page

### GET /order/{id}
Retrieves specific order with all details.

### POST /order
Creates new product order.

**Request Body:**
```json
{
  "customerId": "guid",
  "saleDate": "2024-01-01",
  "status": "Approved",
  "details": [
    { "productId": "guid", "price": 100.00, "quantity": 2 }
  ],
  "employeeId": "guid (optional)"
}
```

### DELETE /order/{id}
Soft-deletes an order (Admin only).

### GET /order/{id}/detail
Retrieves all items for a specific order.

### GET /order/analytics
Retrieves comprehensive order analytics.

**Query Parameters:**
- `days` (int, default: 90) - Number of days to analyze
- `topCustomersLimit` (int, default: 10) - Number of top customers to return

**Analytics Include:**
- Orders grouped by status
- Daily sales trends
- Top customers by spending
- Average order value (average, median, min, max)
- Recent cancelled orders

## Integration with Other Domains

- **Customer**: Associates orders with customers
- **Employee**: Tracks which employee processed the order
- **Product**: References products in order details
- **Stock**: Creates stock movements and updates inventory
- **OrderDetail**: Provides detailed order item information

## Unit Tests

### OrderControllerTests

Tests for order CRUD endpoints and attributes.

### CreateOrderHandlerTests

Tests for order creation including:
- Valid order creation
- Inventory updates
- Stock movement creation

### GetOrderDetailsByOrderIdHandlerTests

Tests for retrieving order details.

