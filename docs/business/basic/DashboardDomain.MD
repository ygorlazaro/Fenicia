# Dashboard Domain

The Dashboard domain provides financial analytics and business intelligence through a comprehensive dashboard system in the Fenicia system.

## Overview

This domain provides functionality for:
- Retrieving financial dashboard with key performance indicators
- Revenue and cost analysis
- Profit margin trend tracking
- Accounts receivable summary
- Daily sales performance tracking

## Business Logic

### Financial Dashboard
The dashboard provides comprehensive business analytics including:

- **KPI Summary**: Core business metrics (revenue, costs, profit, orders, products)
- **Revenue vs Cost**: Analysis comparing revenue to costs over time
- **Profit Margin Trend**: Weekly profit margin trends with direction indicators
- **Accounts Receivable**: Summary of pending and approved orders
- **Daily Sales**: Sales performance for today, this week, and this month with growth comparison

### KPI Calculations
- **Total Revenue**: Sum of all order amounts
- **Total Cost**: Estimated cost (70% of revenue based on order details)
- **Gross Profit**: Revenue minus cost
- **Profit Margin**: Percentage of profit relative to revenue
- **Average Order Value**: Total revenue divided by number of orders
- **Total Stock Value**: Sum of product cost * quantity

### Sales Analysis
- **Daily Sales**: Today's revenue and orders
- **Weekly Sales**: Current week revenue and orders
- **Monthly Sales**: Current month revenue and orders
- **Growth Percentage**: Month-over-month revenue comparison

### Profit Margin Trend
- Analyzes profit margins by week
- Classifies trends as "Improving", "Stable", or "Declining"
- Compares current margin to previous week's margin

### Accounts Receivable
- **Pending Orders**: Orders with Pending status and their total amount
- **Approved Orders**: Orders with Approved status and their total amount

## Components

### Controllers

#### DashboardController
HTTP endpoint controller providing REST API for dashboard operations.

- `GET /dashboard/financial` - Retrieves comprehensive financial dashboard

### Handlers

#### GetFinancialDashboardHandler
Handles generation of financial dashboard data. Aggregates multiple analytics:
- KPI Summary calculation
- Revenue vs Cost analysis
- Profit Margin Trend analysis
- Accounts Receivable calculation
- Daily Sales Summary

### Queries

#### GetFinancialDashboardQuery
Query for retrieving financial dashboard.
- `Days`: Number of days to analyze (default: 90)

### Responses

#### FinancialDashboardResponse
Complete financial dashboard response containing:
- `Kpi`: Key Performance Indicators
- `RevenueVsCost`: Revenue vs Cost analysis
- `ProfitMarginTrend`: Profit margin trends
- `AccountsReceivable`: Accounts receivable summary
- `DailySales`: Daily sales summary

#### KpiSummaryResponse
Key Performance Indicators:
- `TotalRevenue`: Total revenue from all orders
- `TotalCost`: Total estimated cost
- `GrossProfit`: Revenue minus cost
- `ProfitMargin`: Profit margin percentage
- `TotalOrders`: Number of orders
- `TotalProducts`: Number of products
- `AverageOrderValue`: Average revenue per order
- `TotalStockValue`: Total inventory value

#### RevenueVsCostResponse
Revenue vs Cost comparison:
- `Period`: Period name
- `Date`: Date of the period
- `Revenue`: Revenue amount
- `Cost`: Cost amount
- `Profit`: Profit amount (Revenue - Cost)

#### ProfitMarginTrendResponse
Profit margin trend:
- `Period`: Period name (e.g., "Week 12")
- `Date`: Date of the period
- `MarginPercentage`: Profit margin percentage
- `Trend`: Trend direction (Improving, Stable, Declining)

#### AccountsReceivableResponse
Accounts receivable summary:
- `TotalPending`: Total pending order amounts
- `PendingOrdersCount`: Number of pending orders
- `TotalApproved`: Total approved order amounts
- `ApprovedOrdersCount`: Number of approved orders

#### DailySalesSummaryResponse
Daily sales metrics:
- `TodayRevenue`: Today's revenue
- `TodayOrders`: Today's order count
- `WeekRevenue`: This week's revenue
- `WeekOrders`: This week's order count
- `MonthRevenue`: This month's revenue
- `MonthOrders`: This month's order count
- `PreviousMonthRevenue`: Last month's revenue
- `GrowthPercentage`: Month-over-month growth

## Security

- All dashboard endpoints require authentication
- WideEventContext is used for request tracking and logging

## Testing

There are currently no unit tests for the Dashboard domain.
