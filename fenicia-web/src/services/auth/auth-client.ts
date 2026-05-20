import { ApiClient, AUTH_API_BASE_URL } from "../api-client";

/**
 * AuthClient - Base class for authentication microservice
 * Extends ApiClient with auth-specific functionality
 */
export class AuthClient extends ApiClient {
    constructor(baseURL: string = AUTH_API_BASE_URL) {
        super(baseURL);
    }
}

export default AuthClient;
