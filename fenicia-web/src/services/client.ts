import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5144';
export const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || 'http://localhost:5144';

// Storage keys
const TOKEN_KEY = 'auth_token';
const COMPANY_ID_KEY = 'company_id';

/**
 * Abstract base class for API clients
 * Provides authentication, company header, and error handling
 */
export class ApiClient {
  protected client: AxiosInstance;

  constructor(baseURL: string = API_BASE_URL) {
    this.client = axios.create({
      baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  /**
   * Get the stored auth token
   */
  protected getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  /**
   * Set the auth token
   */
  protected setToken(token: string | null): void {
    if (token) {
      localStorage.setItem(TOKEN_KEY, token);
    } else {
      localStorage.removeItem(TOKEN_KEY);
    }
  }

  /**
   * Get the stored company ID
   */
  protected getCompanyId(): string | null {
    return localStorage.getItem(COMPANY_ID_KEY);
  }

  /**
   * Set the company ID
   */
  protected setCompanyId(companyId: string | null): void {
    if (companyId) {
      localStorage.setItem(COMPANY_ID_KEY, companyId);
    } else {
      localStorage.removeItem(COMPANY_ID_KEY);
    }
  }

  /**
   * Clear all auth data
   */
  protected clearAuth(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(COMPANY_ID_KEY);
    localStorage.removeItem('company_name');
  }

  /**
   * Setup request and response interceptors
   */
  protected setupInterceptors(): void {
    // Request interceptor: Add Authorization header and x-company header
    this.client.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = this.getToken();
        const companyId = this.getCompanyId();

        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        if (companyId && config.headers) {
          config.headers['x-company'] = companyId;
        }

        return config;
      },
      (error: AxiosError) => Promise.reject(error)
    );

    // Response interceptor: Handle 401 errors
    this.client.interceptors.response.use(
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

  /**
   * Get the underlying axios instance
   */
  public getClient(): AxiosInstance {
    return this.client;
  }
}

/**
 * Default generic API client for common use cases
 */
export class DefaultApiClient extends ApiClient {
  // You can add specific methods here if needed
}

// Storage utility functions (for backward compatibility)
export const getToken = (): string | null => localStorage.getItem(TOKEN_KEY);

export const setToken = (token: string | null): void => {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
};

export const getCompanyId = (): string | null => localStorage.getItem(COMPANY_ID_KEY);

export const setCompanyId = (companyId: string | null): void => {
  if (companyId) {
    localStorage.setItem(COMPANY_ID_KEY, companyId);
  } else {
    localStorage.removeItem(COMPANY_ID_KEY);
  }
};

export const clearAuth = (): void => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(COMPANY_ID_KEY);
  localStorage.removeItem('company_name');
};

// Default instance for backward compatibility
export const client = new DefaultApiClient().getClient();

export default client;
