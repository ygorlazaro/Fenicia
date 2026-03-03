import { AuthClient } from './auth-client';

/**
 * AuthCompanyClient - Handles company-related operations
 * Implements fetching user companies
 */
export class AuthCompanyClient extends AuthClient {
  constructor(baseURL) {
    super(baseURL);
  }

  /**
   * Get all companies for the logged user
   * GET /company?page=1&perPage=10
   * @param {number} page - Page number
   * @param {number} perPage - Items per page
   * @returns {Promise<GetCompaniesByUserResponse>}
   */
  async getCompaniesByUser(page = 1, perPage = 10) {
    const response = await this.getClient().get('/company', {
      params: { page, perPage }
    });

    return response.data;
  }
}

export default AuthCompanyClient;
