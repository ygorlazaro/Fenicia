import { ApiClient } from './client';

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || 'http://localhost:5144';

// Default company ID for initial login (can be overridden)
const DEFAULT_COMPANY_ID = import.meta.env.VITE_DEFAULT_COMPANY_ID || '00000000-0000-0000-0000-000000000000';

/**
 * AuthClient - Base class for authentication microservice
 * Extends ApiClient with auth-specific functionality
 */
export class AuthClient extends ApiClient {
  constructor(baseURL = AUTH_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Override setupInterceptors to add default company header for auth requests
   */
  setupInterceptors() {
    super.setupInterceptors();

    this.client.interceptors.request.use(
      (config) => {
        const token = this.getToken();
        let companyId = this.getCompanyId();

        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        // Use stored company ID or default for anonymous requests
        if (companyId || DEFAULT_COMPANY_ID) {
          config.headers['x-company'] = companyId || DEFAULT_COMPANY_ID;
        }

        return config;
      },
      (error) => Promise.reject(error)
    );

    this.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          this.clearAuth();
          window.location.href = '/';
        }
        return Promise.reject(error);
      }
    );
  }
}

export default AuthClient;
