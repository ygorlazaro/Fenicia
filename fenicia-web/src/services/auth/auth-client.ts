import { AxiosError, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import { ApiClient, AUTH_API_BASE_URL } from '../api-client';

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

        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
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
