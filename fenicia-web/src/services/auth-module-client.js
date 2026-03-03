import { ApiClient } from './client';

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || 'http://localhost:5001/api';

/**
 * AuthModuleClient - Handles module-related operations
 * Implements fetching available modules
 */
export class AuthModuleClient extends ApiClient {
  constructor(baseURL = AUTH_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Get all available modules
   * GET /module?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<Pagination<List<GetModuleResponse>>>}
   */
  async getModules(page = 1, perPage = 50) {
    const response = await this.getClient().get('/module', {
      params: { page, perPage }
    });

    return response.data;
  }
}

export default AuthModuleClient;
