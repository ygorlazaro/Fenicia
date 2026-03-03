import { AuthClient } from './auth-client';
import { UserProfileResponse } from '../types';
import { AUTH_API_BASE_URL } from './client';

/**
 * AuthProfileClient - Handles user profile operations
 * 
 * Microservice: Authentication
 * Base URL: http://localhost:5144 (from VITE_AUTH_API_BASE_URL)
 * Routes: /subscription/profile
 */
export class AuthProfileClient extends AuthClient {
  constructor(baseURL?: string) {
    super(baseURL || AUTH_API_BASE_URL);
  }

  /**
   * Get current user profile with companies and subscriptions
   * GET /subscription/profile
   * @returns Promise<UserProfileResponse>
   */
  async getProfile(): Promise<UserProfileResponse> {
    const response = await this.getClient().get<UserProfileResponse>('/subscription/profile');
    return response.data;
  }
}

export default AuthProfileClient;
