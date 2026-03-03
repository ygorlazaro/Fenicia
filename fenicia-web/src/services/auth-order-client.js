import { ApiClient } from './client';

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || 'http://localhost:5001/api';

/**
 * AuthOrderClient - Handles order/subscription operations
 * Implements creating new orders
 */
export class AuthOrderClient extends ApiClient {
  constructor(baseURL = AUTH_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Create a new subscription order
   * POST /order
   * @param {Object} orderData - Order data
   * @param {Guid[]} orderData.modules - Array of module IDs to subscribe
   * @returns {Promise<CreateNewOrderResponse>}
   */
  async createOrder(orderData) {
    const response = await this.getClient().post('/order', {
      modules: orderData.modules
    });

    return response.data;
  }
}

export default AuthOrderClient;
