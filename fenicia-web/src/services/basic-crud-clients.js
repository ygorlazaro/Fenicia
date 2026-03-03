import {ApiClient} from "src/services/client";

const BASIC_API_BASE_URL = import.meta.env.VITE_BASIC_API_BASE_URL || 'http://localhost:5002/api';

/**
 * Basic Customer Client - Handles customer CRUD operations
 */
export class BasicCustomerClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/customer', { params: { page, perPage } });
    return response.data;
  }

  async getById(id) {
    const response = await this.getClient().get(`/customer/${id}`);
    return response.data;
  }

  async create(customer) {
    const response = await this.getClient().post('/customer', customer);
    return response.data;
  }

  async update(id, customer) {
    const response = await this.getClient().patch(`/customer/${id}`, customer);
    return response.data;
  }

  async delete(id) {
    await this.getClient().delete(`/customer/${id}`);
  }
}

/**
 * Basic Supplier Client - Handles supplier CRUD operations
 */
export class BasicSupplierClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/supplier', { params: { page, perPage } });
    return response.data;
  }

  async getById(id) {
    const response = await this.getClient().get(`/supplier/${id}`);
    return response.data;
  }

  async create(supplier) {
    const response = await this.getClient().post('/supplier', supplier);
    return response.data;
  }

  async update(id, supplier) {
    const response = await this.getClient().patch(`/supplier/${id}`, supplier);
    return response.data;
  }

  async delete(id) {
    await this.getClient().delete(`/supplier/${id}`);
  }
}

/**
 * Basic Product Category Client - Handles product category CRUD operations
 */
export class BasicProductCategoryClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll() {
    const response = await this.getClient().get('/productcategory');
    return response.data;
  }

  async getById(id) {
    const response = await this.getClient().get(`/productcategory/${id}`);
    return response.data;
  }

  async create(category) {
    const response = await this.getClient().post('/productcategory', category);
    return response.data;
  }

  async update(id, category) {
    const response = await this.getClient().patch(`/productcategory/${id}`, category);
    return response.data;
  }

  async delete(id) {
    await this.getClient().delete(`/productcategory/${id}`);
  }

  async getProductsByCategory(categoryId, page = 1, perPage = 10) {
    const response = await this.getClient().get(`/productcategory/${categoryId}/product`, {
      params: { page, perPage }
    });
    return response.data;
  }
}

/**
 * Basic Product Client - Handles product CRUD operations
 */
export class BasicProductClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/product', { params: { page, perPage } });
    return response.data;
  }

  async getById(id) {
    const response = await this.getClient().get(`/product/${id}`);
    return response.data;
  }

  async create(product) {
    const response = await this.getClient().post('/product', product);
    return response.data;
  }

  async update(id, product) {
    const response = await this.getClient().patch(`/product/${id}`, product);
    return response.data;
  }

  async delete(id) {
    await this.getClient().delete(`/product/${id}`);
  }
}

/**
 * Basic Inventory Client - Handles inventory operations
 */
export class BasicInventoryClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(page = 1, perPage = 10) {
    const response = await this.getClient().get('/inventory', { params: { page, perPage } });
    return response.data;
  }

  async getByProduct(productId) {
    const response = await this.getClient().get(`/product/${productId}`);
    return response.data;
  }

  async getByCategory(categoryId) {
    const response = await this.getClient().get(`/category/${categoryId}`);
    return response.data;
  }
}

/**
 * Basic Stock Movement Client - Handles stock movement operations
 */
export class BasicStockMovementClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async getAll(startDate, endDate, page = 1, perPage = 10) {
    const response = await this.getClient().get('/stockmovement', {
      params: { startDate, endDate, page, perPage }
    });
    return response.data;
  }

  async create(movement) {
    const response = await this.getClient().post('/stockmovement', movement);
    return response.data;
  }

  async update(id, movement) {
    const response = await this.getClient().patch(`/stockmovement/${id}`, movement);
    return response.data;
  }
}

/**
 * Basic Order Client - Handles order operations
 */
export class BasicOrderClient extends ApiClient {
  constructor(baseURL = BASIC_API_BASE_URL) {
    super(baseURL);
  }

  async create(order) {
    const response = await this.getClient().post('/order', order);
    return response.data;
  }

  async getDetails(orderId) {
    const response = await this.getClient().get(`/order/${orderId}/detail`);
    return response.data;
  }
}

export default {
  BasicCustomerClient,
  BasicSupplierClient,
  BasicProductCategoryClient,
  BasicProductClient,
  BasicInventoryClient,
  BasicStockMovementClient,
  BasicOrderClient
};
