import { ApiClient, BASIC_API_BASE_URL } from '../client';
import { Customer, Supplier, ProductCategory, Product, Inventory, StockMovement, Order, OrderItem } from '../types';

interface PaginationResponse<T> {
  data: T[];
  total: number;
  page: number;
  perPage: number;
  pages: number;
}

/**
 * Basic Customer Client - Handles customer CRUD operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /customer
 */
export class BasicCustomerClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<PaginationResponse<Customer> | Customer[]> {
    const response = await this.getClient().get<PaginationResponse<Customer>>('/customer', { params: { page, perPage } });
    return response.data;
  }

  async getById(id: string): Promise<Customer> {
    const response = await this.getClient().get<Customer>(`/customer/${id}`);
    return response.data;
  }

  async create(customer: Partial<Customer>): Promise<Customer> {
    const response = await this.getClient().post<Customer>('/customer', customer);
    return response.data;
  }

  async update(id: string, customer: Partial<Customer>): Promise<Customer> {
    const response = await this.getClient().patch<Customer>(`/customer/${id}`, customer);
    return response.data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/customer/${id}`);
  }
}

/**
 * Basic Supplier Client - Handles supplier CRUD operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /supplier
 */
export class BasicSupplierClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<PaginationResponse<Supplier> | Supplier[]> {
    const response = await this.getClient().get<PaginationResponse<Supplier>>('/supplier', { params: { page, perPage } });
    return response.data;
  }

  async getById(id: string): Promise<Supplier> {
    const response = await this.getClient().get<Supplier>(`/supplier/${id}`);
    return response.data;
  }

  async create(supplier: Partial<Supplier>): Promise<Supplier> {
    const response = await this.getClient().post<Supplier>('/supplier', supplier);
    return response.data;
  }

  async update(id: string, supplier: Partial<Supplier>): Promise<Supplier> {
    const response = await this.getClient().patch<Supplier>(`/supplier/${id}`, supplier);
    return response.data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/supplier/${id}`);
  }
}

/**
 * Basic Product Category Client - Handles product category CRUD operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /productcategory
 */
export class BasicProductCategoryClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(): Promise<ProductCategory[]> {
    const response = await this.getClient().get<ProductCategory[]>('/productcategory');
    return response.data;
  }

  async getById(id: string): Promise<ProductCategory> {
    const response = await this.getClient().get<ProductCategory>(`/productcategory/${id}`);
    return response.data;
  }

  async create(category: Partial<ProductCategory>): Promise<ProductCategory> {
    const response = await this.getClient().post<ProductCategory>('/productcategory', category);
    return response.data;
  }

  async update(id: string, category: Partial<ProductCategory>): Promise<ProductCategory> {
    const response = await this.getClient().patch<ProductCategory>(`/productcategory/${id}`, category);
    return response.data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/productcategory/${id}`);
  }

  async getProductsByCategory(categoryId: string, page: number = 1, perPage: number = 10): Promise<PaginationResponse<Product> | Product[]> {
    const response = await this.getClient().get<PaginationResponse<Product>>(`/productcategory/${categoryId}/product`, {
      params: { page, perPage }
    });
    return response.data;
  }
}

/**
 * Basic Product Client - Handles product CRUD operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /product
 */
export class BasicProductClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<PaginationResponse<Product> | Product[]> {
    const response = await this.getClient().get<PaginationResponse<Product>>('/product', { params: { page, perPage } });
    return response.data;
  }

  async getById(id: string): Promise<Product> {
    const response = await this.getClient().get<Product>(`/product/${id}`);
    return response.data;
  }

  async create(product: Partial<Product>): Promise<Product> {
    const response = await this.getClient().post<Product>('/product', product);
    return response.data;
  }

  async update(id: string, product: Partial<Product>): Promise<Product> {
    const response = await this.getClient().patch<Product>(`/product/${id}`, product);
    return response.data;
  }

  async delete(id: string): Promise<void> {
    await this.getClient().delete(`/product/${id}`);
  }
}

/**
 * Basic Inventory Client - Handles inventory operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /inventory
 */
export class BasicInventoryClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page: number = 1, perPage: number = 10): Promise<PaginationResponse<Inventory> | Inventory[]> {
    const response = await this.getClient().get<PaginationResponse<Inventory>>('/inventory', { params: { page, perPage } });
    return response.data;
  }

  async getByProduct(productId: string): Promise<Inventory> {
    const response = await this.getClient().get<Inventory>(`/product/${productId}`);
    return response.data;
  }

  async getByCategory(categoryId: string): Promise<Inventory> {
    const response = await this.getClient().get<Inventory>(`/category/${categoryId}`);
    return response.data;
  }
}

/**
 * Basic Stock Movement Client - Handles stock movement operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /stockmovement
 */
export class BasicStockMovementClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(startDate?: string, endDate?: string, page: number = 1, perPage: number = 10): Promise<PaginationResponse<StockMovement> | StockMovement[]> {
    const response = await this.getClient().get<PaginationResponse<StockMovement>>('/stockmovement', {
      params: { startDate, endDate, page, perPage }
    });
    return response.data;
  }

  async create(movement: Partial<StockMovement>): Promise<StockMovement> {
    const response = await this.getClient().post<StockMovement>('/stockmovement', movement);
    return response.data;
  }

  async update(id: string, movement: Partial<StockMovement>): Promise<StockMovement> {
    const response = await this.getClient().patch<StockMovement>(`/stockmovement/${id}`, movement);
    return response.data;
  }
}

/**
 * Basic Order Client - Handles order operations
 * 
 * Microservice: Basic Module
 * Base URL: http://localhost:5083 (from VITE_BASIC_API_BASE_URL)
 * Routes: /order
 */
export class BasicOrderClient extends ApiClient {
  constructor(baseURL: string = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async create(order: { items: OrderItem[] }): Promise<Order> {
    const response = await this.getClient().post<Order>('/order', order);
    return response.data;
  }

  async getDetails(orderId: string): Promise<OrderItem[]> {
    const response = await this.getClient().get<OrderItem[]>(`/order/${orderId}/detail`);
    return response.data;
  }
}

// Export all clients
export {
  BasicCustomerClient,
  BasicSupplierClient,
  BasicProductCategoryClient,
  BasicProductClient,
  BasicInventoryClient,
  BasicStockMovementClient,
  BasicOrderClient
};

export default {
  BasicCustomerClient,
  BasicSupplierClient,
  BasicProductCategoryClient,
  BasicProductClient,
  BasicInventoryClient,
  BasicStockMovementClient,
  BasicOrderClient
};
