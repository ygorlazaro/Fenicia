import { AuthClient, AUTH_API_BASE_URL } from './auth-client';
import { setToken, setCompanyId } from './client';
import { TokenResponse } from '../types';

interface GenerateTokenCredentials {
  email: string;
  password: string;
  cnpj?: string;
}

interface RefreshTokenData {
  userId: string;
  refreshToken: string;
}

/**
 * AuthTokenClient - Handles token authentication operations
 * Implements login and token refresh functionality
 * 
 * Microservice: Authentication
 * Base URL: http://localhost:5144 (from VITE_AUTH_API_BASE_URL)
 * Routes: /token, /token/refresh
 */
export class AuthTokenClient extends AuthClient {
  constructor(baseURL?: string) {
    super(baseURL || AUTH_API_BASE_URL);
  }

  /**
   * Authenticate user and generate token
   * POST /token
   * @param credentials - User credentials
   * @returns Promise<TokenResponse>
   */
  async generateToken(credentials: GenerateTokenCredentials): Promise<TokenResponse> {
    const response = await this.getClient().post<TokenResponse>('/token', credentials);
    const data = response.data;

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
   * @param requestData - Refresh token data
   * @returns Promise<TokenResponse>
   */
  async refreshToken(requestData: RefreshTokenData): Promise<TokenResponse> {
    const response = await this.getClient().post<TokenResponse>('/token/refresh', requestData);
    const data = response.data;

    // Persist new access token
    if (data.accessToken) {
      setToken(data.accessToken);
    }

    return data;
  }
}

export default AuthTokenClient;
