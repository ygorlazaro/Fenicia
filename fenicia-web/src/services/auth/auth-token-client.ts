import { AxiosResponse } from 'axios';
import { GenerateTokenQuery, TokenResponse, ValidateTokenQuery } from '../../types/auth-types';
import { setCompanyId, setToken } from '../api-client';
import { AuthClient } from './auth-client';

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
   * @returns {Promise<TokenResponse>}
   */
  async generateToken(credentials: GenerateTokenQuery): Promise<TokenResponse> {
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
   * @returns {Promise<TokenResponse>}
   */
  async refreshToken(requestData: ValidateTokenQuery): Promise<TokenResponse> {
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
