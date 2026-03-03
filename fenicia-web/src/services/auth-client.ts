import { ApiClient, AUTH_API_BASE_URL } from './client';

/**
 * AuthClient - Base class for authentication microservice
 * Extends ApiClient with auth-specific functionality
 * 
 * Microservice: Authentication & Authorization
 * Base URL: http://localhost:5144 (from VITE_AUTH_API_BASE_URL)
 * Routes: /token, /register, /company, /forgotpassword, etc.
 */
export class AuthClient extends ApiClient {
  constructor(baseURL: string = AUTH_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Override setupInterceptors to add default company header for auth requests
   */
  setupInterceptors(): void {
    this.client.interceptors.request.use(
      (config) => {
        const token = this.getToken();
        let companyId = this.getCompanyId();

        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        // Use stored company ID or default for anonymous requests
        if (companyId || import.meta.env.VITE_DEFAULT_COMPANY_ID) {
          config.headers['x-company'] = companyId || import.meta.env.VITE_DEFAULT_COMPANY_ID;
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
