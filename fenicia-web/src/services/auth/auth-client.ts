import { ApiClient } from "../api-client.ts";
import { store } from "../store.ts";
import { setCredentials } from "../features/auth/authSlice.ts";

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL || "http://localhost:5001/api";

export class AuthClient extends ApiClient {
    constructor(baseURL: string = AUTH_API_BASE_URL) {
        super(baseURL);
    }

    public setAuthData(token: string, refreshToken: string, user: { id: string; email: string; name: string; companyId?: string }): void {
        store.dispatch(setCredentials({ token, refreshToken, user }));
    }
}

export default AuthClient;
