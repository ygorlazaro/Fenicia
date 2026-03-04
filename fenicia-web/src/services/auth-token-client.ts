import { AuthClient } from './auth-client';
import { setToken, setCompanyId } from './client';
import { AxiosResponse } from 'axios';

/**
 * AuthTokenClient - Handles token authentication operations
 * Implements login and token refresh functionality
 */
export class AuthTokenClient extends AuthClient {
  constructor(baseURL?: string) {
    super(baseURL);
  }

  /**
   * Authenticate user and generate token
   * POST /token
   * @param {Object} credentials - User credentials
   * @param {string} credentials.email - User email
   * @param {string} [credentials.password] - User password (optional depending on implementation)
   * @returns {Promise<any>}
   */
  async generateToken(credentials: { email: string; password?: string }): Promise<any> {
    const response = await this.getClient().post('/token', credentials);
    const data = (response as AxiosResponse).data;

    // Persist access token
    if (data.accessToken) {
      setToken(data.accessToken);
    }

    // Persist company ID if returned
    if (data.user?.companyId) {
      setCompanyId(data.user.companyId);
    }

    return data;
  }

  /**
   * Refresh access token using refresh token
   * POST /token/refresh
   * @param {Object} requestData - Refresh token data
   * @param {string} requestData.userId - User ID
   * @param {string} requestData.refreshToken - Refresh token
   * @returns {Promise<any>}
   */
  async refreshToken(requestData: { userId: string; refreshToken: string }): Promise<any> {
    const response = await this.getClient().post('/token/refresh', requestData);
    const data = (response as AxiosResponse).data;

    // Persist new access token
    if (data.accessToken) {
      setToken(data.accessToken);
    }

    return data;
  }
}

export default AuthTokenClient;
