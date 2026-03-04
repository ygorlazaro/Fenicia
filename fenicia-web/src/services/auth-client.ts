import { ApiClient, AUTH_API_BASE_URL } from './client';
import { InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';

// Default company ID for initial login (can be overridden)
const DEFAULT_COMPANY_ID = import.meta.env.VITE_DEFAULT_COMPANY_ID || '00000000-0000-0000-0000-000000000000';

/**
 * AuthClient - Base class for authentication microservice
 * Extends ApiClient with auth-specific functionality
 */
export class AuthClient extends ApiClient {
  constructor(baseURL: string = AUTH_API_BASE_URL) {
    super(baseURL);
  }

  /**
   * Override setupInterceptors to add default company header for auth requests
   */
  protected setupInterceptors(): void {
    super.setupInterceptors();

    this.getClient().interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = this.getToken();
        let companyId = this.getCompanyId();

        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        // Use stored company ID or default for anonymous requests
        if ((companyId || DEFAULT_COMPANY_ID) && config.headers) {
          config.headers['x-company'] = companyId || DEFAULT_COMPANY_ID;
        }

        return config;
      },
      (error: AxiosError) => Promise.reject(error)
    );

    this.getClient().interceptors.response.use(
      (response: AxiosResponse) => response,
      (error: AxiosError) => {
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
