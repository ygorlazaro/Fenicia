import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5144';

// Storage keys
const TOKEN_KEY = 'auth_token';
const COMPANY_ID_KEY = 'company_id';

/**
 * Abstract base class for API clients
 * Provides authentication, company header, and error handling
 */
export class ApiClient {
  constructor(baseURL = API_BASE_URL) {
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
  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  /**
   * Set the auth token
   */
  setToken(token) {
    if (token) {
      localStorage.setItem(TOKEN_KEY, token);
    } else {
      localStorage.removeItem(TOKEN_KEY);
    }
  }

  /**
   * Get the stored company ID
   */
  getCompanyId() {
    return localStorage.getItem(COMPANY_ID_KEY);
  }

  /**
   * Set the company ID
   */
  setCompanyId(companyId) {
    if (companyId) {
      localStorage.setItem(COMPANY_ID_KEY, companyId);
    } else {
      localStorage.removeItem(COMPANY_ID_KEY);
    }
  }

  /**
   * Clear all auth data
   */
  clearAuth() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(COMPANY_ID_KEY);
    localStorage.removeItem('company_name');
  }

  /**
   * Setup request and response interceptors
   */
  setupInterceptors() {
    // Request interceptor: Add Authorization header and x-company header
    this.client.interceptors.request.use(
      (config) => {
        const token = this.getToken();
        const companyId = this.getCompanyId();

        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }

        if (companyId) {
          config.headers['x-company'] = companyId;
        }

        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor: Handle 401 errors
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

  /**
   * Get the underlying axios instance
   */
  getClient() {
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
export const getToken = () => localStorage.getItem(TOKEN_KEY);

export const setToken = (token) => {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
};

export const getCompanyId = () => localStorage.getItem(COMPANY_ID_KEY);

export const setCompanyId = (companyId) => {
  if (companyId) {
    localStorage.setItem(COMPANY_ID_KEY, companyId);
  } else {
    localStorage.removeItem(COMPANY_ID_KEY);
  }
};

export const clearAuth = () => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(COMPANY_ID_KEY);
  localStorage.removeItem('company_name');
};

// Default instance for backward compatibility
export const client = new DefaultApiClient().getClient();

export default client;
