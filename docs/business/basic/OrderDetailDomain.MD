# OrderDetail Domain

## Overview

The OrderDetail domain manages individual line items (products) within an order. Each order detail represents a specific product purchased in an order, including its price and quantity.

Related documentation:
- See [OrderDomain](./OrderDomain.MD) for orders
- See [ProductDomain](./ProductDomain.MD) for products

## Architecture

### Handlers

| Handler | Description |
|---------|-------------|
| `GetOrderDetailsByOrderIdHandler` | Retrieves all order detail items for a specific order |

### Queries

| Query | Description |
|-------|-------------|
| `GetOrderDetailsByOrderIdQuery` | Request to get all details for a specific order by order ID |

### Responses

| Response | Description |
|----------|-------------|
| `GetOrderDetailsByOrderIdResponse` | Contains the list of order detail items |

## Usage

Order details represent individual products in an order with:
- **ProductId** - Reference to the product
- **Price** - Unit price at time of order
- **Quantity** - Number of units purchased
- **OrderId** - Reference to the parent order

## Data Model

The `BasicOrderDetail` entity stores:
- Unique identifier (Id)
- Order reference (OrderId)
- Product reference (ProductId)
- Unit price (Price)
- Quantity (Quantity)

Each order detail is linked to a parent order via `OrderId` and a product via `ProductId`.
