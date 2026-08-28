import axios, { AxiosError, AxiosInstance, AxiosResponse, InternalAxiosRequestConfig } from "axios";
import { store } from "../store.ts";
import { setCredentials, logout } from "../features/auth/authSlice.ts";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5144";
export const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || "http://localhost:5144";

// Storage keys
const TOKEN_KEY = "auth_token";
const COMPANY_ID_KEY = "company_id";

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
                "Content-Type": "application/json"
            }
        });

        this.setupInterceptors();
    }

    /**
     * Get the stored auth token
     */
    public getToken(): string | null {
        return store.getState().auth.token || localStorage.getItem(TOKEN_KEY);
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
    public getCompanyId(): string | null {
        return store.getState().auth.companyId || localStorage.getItem(COMPANY_ID_KEY);
    }

    /**
     * Set the company ID
     */
    public setCompanyId(companyId: string | null): void {
        if (companyId) {
            localStorage.setItem(COMPANY_ID_KEY, companyId);
        } else {
            localStorage.removeItem(COMPANY_ID_KEY);
        }
    }

    /**
     * Clear all auth data
     */
    public clearAuth(): void {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(COMPANY_ID_KEY);
        localStorage.removeItem("company_name");
        store.dispatch(logout());
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
                    config.headers["CompanyId"] = companyId;
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
                    window.location.href = "/";
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
