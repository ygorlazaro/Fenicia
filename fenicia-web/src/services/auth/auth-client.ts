import { ApiClient } from "../api-client";

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || "http://localhost:5001/api";

export class AuthClient extends ApiClient {
    constructor(baseURL: string = AUTH_API_BASE_URL) {
        super(baseURL);
    }
}

export default AuthClient;
