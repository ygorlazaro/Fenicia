import { AxiosResponse } from "axios";
import { IPagination } from "../../types/index.ts";
import { GetCompaniesByUserResponse } from "../../types/auth/get-companies-by-user-response";
import { AuthClient } from "./auth-client.ts";

/**
 * AuthCompanyClient - Handles company-related operations
 * Implements fetching user companies
 */
export class AuthCompanyClient extends AuthClient {
    constructor(baseURL?: string) {
        super(baseURL);
    }

    /**
     * Get all companies for the logged user
     * GET /company?page=1&perPage=10
     * @param {number} page - Page number
     * @param {number} perPage - Items per page
     * @returns {Promise<GetCompaniesByUserResponse[]>}
     */
    async getCompaniesByUser(page: number = 1, perPage: number = 10): Promise<IPagination<GetCompaniesByUserResponse>> {
        const response = await this.getClient().get("/company", {
            params: { page, perPage }
        });

        return (response as AxiosResponse).data;
    }
}

export default AuthCompanyClient;
