import { AxiosResponse } from "axios";
import { CreateNewUserCommand } from "../../types/auth/create-new-user-command";
import { CreateNewUserResponse } from "../../types/auth/create-new-user-response";
import { AuthClient } from "./auth-client";

/**
 * AuthRegisterClient - Handles user registration operations
 * Implements user registration functionality
 */
export class AuthRegisterClient extends AuthClient {
    constructor(baseURL?: string) {
        super(baseURL);
    }

    /**
     * Register a new user
     * POST /register
     * @param {Object} userData - User registration data
     * @param {string} userData.email - User email
     * @param {string} userData.password - User password
     * @param {string} userData.name - User name
     * @param {Object} userData.company - Company data
     * @param {string} userData.company.cnpj - Company CNPJ
     * @param {string} userData.company.name - Company name
     * @param {string} [userData.company.timeZone] - Company time zone
     * @returns {Promise<CreateNewUserResponse>}
     */
    async register(userData: CreateNewUserCommand): Promise<CreateNewUserResponse> {
        const response = await this.getClient().post("/register", userData);

        return (response as AxiosResponse).data;
    }
}

export default AuthRegisterClient;
