import { AxiosResponse } from "axios";
import { GetUserProfileResponse } from "../../types/auth/get-user-profile-response";
import { AuthClient } from "./auth-client";

/**
 * AuthProfileClient - Handles user profile operations
 *
 * Microservice: Authentication
 * Base URL: http://localhost:5144 (from VITE_AUTH_API_BASE_URL)
 * Routes: /subscription/profile
 */
export class AuthSubscriptionClient extends AuthClient {
    constructor(baseURL?: string) {
        super(baseURL);
    }

    /**
     * Get full profile including subscriptions and modules
     * GET /subscription/profile
     * @returns {Promise<GetUserProfileResponse>} Profile data
     */
    async getProfile(): Promise<GetUserProfileResponse> {
        const response = await this.getClient().get("/subscription/profile");
        return (response as AxiosResponse).data;
    }
}

export default AuthSubscriptionClient;
