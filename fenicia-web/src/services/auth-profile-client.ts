import { AuthClient } from './auth-client';
import { AxiosResponse } from 'axios';

/**
 * AuthProfileClient - Handles user profile operations
 *
 * Microservice: Authentication
 * Base URL: http://localhost:5144 (from VITE_AUTH_API_BASE_URL)
 * Routes: /subscription/profile
 */
export class AuthProfileClient extends AuthClient {
  constructor(baseURL?: string) {
    super(baseURL);
  }

  /**
   * Get current user profile with companies and subscriptions
   * GET /subscription/profile
   * @returns {Promise<any>}
   */
  async getProfile(): Promise<any> {
    const response = await this.getClient().get('/subscription/profile');
    return (response as AxiosResponse).data;
  }
}

export default AuthProfileClient;
