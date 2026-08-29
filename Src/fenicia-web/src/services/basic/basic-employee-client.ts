import { AxiosResponse } from "axios";
import type { IPagination } from "../../types/index.ts";
import type { AddEmployeeCommand } from "../../types/basic/employee/add-employee-command";
import type { AddEmployeeResponse } from "../../types/basic/employee/add-employee-response";
import type { GetAllEmployeeResponse } from "../../types/basic/employee/get-all-employee-response";
import type { GetEmployeeByIdResponse } from "../../types/basic/employee/get-employee-by-id-response";
import type { UpdateEmployeeCommand } from "../../types/basic/employee/update-employee-command";
import type { UpdateEmployeeResponse } from "../../types/basic/employee/update-employee-response";
import type { DataSourceItem } from "../../types/basic/product-category/add-product-category-command";

import { EmployeePerformance } from "../../types/basic/employee/employee-performance";
import { ApiClient } from "../api-client.ts";
import { BASIC_API_BASE_URL } from "./basic-product-client";

/**
 * BasicEmployeeClient - Handles employee CRUD operations
 */
export class BasicEmployeeClient extends ApiClient {
    constructor(baseURL: string = BASIC_API_BASE_URL) {
        super(baseURL);
    }

    /**
     * Get all employees with pagination
     * GET /employee?page=1&perPage=10
     */
    async getAll(page: number = 1, perPage: number = 10): Promise<IPagination<GetAllEmployeeResponse>> {
        const response = await this.getClient().get("/employee", {
            params: { page, perPage }
        });

        return (response as AxiosResponse).data;
    }

    /**
     * Get employee by ID
     * GET /employee/:id
     * @param id Employee ID
     */
    async getById(id: string): Promise<GetEmployeeByIdResponse> {
        const response = await this.getClient().get(`/employee/${id}`);
        return (response as AxiosResponse).data;
    }

    /**
     * Create new employee
     * POST /employee
     */
    async create(employee: AddEmployeeCommand): Promise<AddEmployeeResponse> {
        const response = await this.getClient().post("/employee", employee);
        return (response as AxiosResponse).data;
    }

    /**
     * Update employee
     * PATCH /employee/:id
     */
    async update(id: string, employee: UpdateEmployeeCommand): Promise<UpdateEmployeeResponse> {
        const response = await this.getClient().patch(`/employee/${id}`, employee);
        return (response as AxiosResponse).data;
    }

    /**
     * Delete employee
     * DELETE /employee/:id
     * @param {string} id - Employee ID
     * @returns {Promise<void>}
     */
    async delete(id: string): Promise<void> {
        await this.getClient().delete(`/employee/${id}`);
    }

    /**
     * Get all positions for data source
     * GET /datasource/position
     */
    async getPositions(): Promise<DataSourceItem[]> {
        const response = await this.getClient().get("/datasource/position");
        return (response as AxiosResponse).data;
    }

    async getPerformance(days?: number, topLimit?: number): Promise<EmployeePerformance> {
        const params: any = {};
        if (days !== undefined) params.days = days;
        if (topLimit !== undefined) params.topLimit = topLimit;

        const response = await this.getClient().get("/employee/performance", { params });
        return (response as AxiosResponse).data;
    }
}

export default BasicEmployeeClient;
